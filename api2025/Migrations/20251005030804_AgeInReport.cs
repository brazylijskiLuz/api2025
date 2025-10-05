using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api2025.Migrations
{
    /// <inheritdoc />
    public partial class AgeInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "report",
                type: "integer",
                nullable: false,
                defaultValue: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age",
                table: "report");
        }
    }
}
