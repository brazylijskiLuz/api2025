using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api2025.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usage_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expected_pension = table.Column<decimal>(type: "numeric", nullable: false),
                    sex = table.Column<int>(type: "integer", nullable: false),
                    salary_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    considered_sick_leave = table.Column<bool>(type: "boolean", nullable: false),
                    account_balance = table.Column<decimal>(type: "numeric", nullable: false),
                    sub_account_balance = table.Column<decimal>(type: "numeric", nullable: false),
                    pension = table.Column<decimal>(type: "numeric", nullable: false),
                    real_pension = table.Column<decimal>(type: "numeric", nullable: false),
                    postal_code = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report");
        }
    }
}
