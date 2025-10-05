using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api2025.Migrations
{
    /// <inheritdoc />
    public partial class PostCodeRealtion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "report");

            migrationBuilder.AddColumn<Guid>(
                name: "postal_code_id",
                table: "report",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_report_postal_code_id",
                table: "report",
                column: "postal_code_id");

            migrationBuilder.AddForeignKey(
                name: "fk_report_post_code_postal_code_id",
                table: "report",
                column: "postal_code_id",
                principalTable: "post_code",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_report_post_code_postal_code_id",
                table: "report");

            migrationBuilder.DropIndex(
                name: "ix_report_postal_code_id",
                table: "report");

            migrationBuilder.DropColumn(
                name: "postal_code_id",
                table: "report");

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "report",
                type: "text",
                nullable: true);
        }
    }
}
