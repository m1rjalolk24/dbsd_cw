using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSD_CW2.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameNameToFirstName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "FirstName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Products",
                newName: "Name");
        }
    }
}
