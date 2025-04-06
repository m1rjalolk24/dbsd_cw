using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSD_CW2.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLastNameToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Products");
        }
    }
}
