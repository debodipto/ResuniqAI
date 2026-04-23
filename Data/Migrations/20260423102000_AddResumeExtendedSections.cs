using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResuniqAI.Data.Migrations
{
    public partial class AddResumeExtendedSections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AchievementDetails",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Achievements",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificationDetails",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LeadershipActivityDetails",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LeadershipAndActivities",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectDetails",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Projects",
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

            migrationBuilder.AddColumn<string>(
                name: "ReferenceDetails",
                table: "Resumes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AchievementDetails", table: "Resumes");
            migrationBuilder.DropColumn(name: "Achievements", table: "Resumes");
            migrationBuilder.DropColumn(name: "AdditionalInformation", table: "Resumes");
            migrationBuilder.DropColumn(name: "CertificationDetails", table: "Resumes");
            migrationBuilder.DropColumn(name: "Certifications", table: "Resumes");
            migrationBuilder.DropColumn(name: "LeadershipActivityDetails", table: "Resumes");
            migrationBuilder.DropColumn(name: "LeadershipAndActivities", table: "Resumes");
            migrationBuilder.DropColumn(name: "ProjectDetails", table: "Resumes");
            migrationBuilder.DropColumn(name: "Projects", table: "Resumes");
            migrationBuilder.DropColumn(name: "Reference", table: "Resumes");
            migrationBuilder.DropColumn(name: "ReferenceDetails", table: "Resumes");
        }
    }
}
