using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResuniqAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeTemplateKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "Resumes",
                newName: "TemplateKey");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpireDate",
                table: "Subscriptions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemplateKey",
                table: "Resumes",
                newName: "Reference");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpireDate",
                table: "Subscriptions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }
    }
}
