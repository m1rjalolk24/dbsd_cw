using Microsoft.EntityFrameworkCore;
using DBSD_CW2.Data;

var builder = WebApplication.CreateBuilder(args);

// Set up DataDirectory
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "Database");
AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get connection string and replace %CONTENTROOTPATH% placeholder
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString.Contains("%CONTENTROOTPATH%"))
{
    // Go up one directory from ContentRootPath to reach the project root
    string projectRootPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, ".."));
    connectionString = connectionString.Replace("%CONTENTROOTPATH%", projectRootPath);
}

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly("DBSD_CW2.Web")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Only use HTTPS redirection in production
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
