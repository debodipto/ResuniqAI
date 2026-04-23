using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResuniqAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Github",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedIn",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Portfolio",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Github",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "LinkedIn",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Portfolio",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Resumes");
        }
    }
}
