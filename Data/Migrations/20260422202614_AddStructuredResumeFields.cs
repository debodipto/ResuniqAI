using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResuniqAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStructuredResumeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DegreeName",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EducationDetails",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmploymentAchievements",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmploymentDuration",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmploymentLocation",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmploymentResponsibilities",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gpa",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PassingYear",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PositionTitle",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UniversityName",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "DegreeName",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EducationDetails",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EmploymentAchievements",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EmploymentDuration",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EmploymentLocation",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EmploymentResponsibilities",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Gpa",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PassingYear",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PositionTitle",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "UniversityName",
                table: "Resumes");
        }
    }
}
