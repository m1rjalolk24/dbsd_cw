using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSD_CW2.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceAndDurationToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerMonth",
                table: "Categories",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PricePerMonth",
                table: "Categories");
        }
    }
}
