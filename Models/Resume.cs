namespace ResuniqAI.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string LinkedIn { get; set; } = "";
        public string Github { get; set; } = "";
        public string Portfolio { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Skills { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string PositionTitle { get; set; } = "";
        public string EmploymentDuration { get; set; } = "";
        public string EmploymentLocation { get; set; } = "";
        public string EmploymentResponsibilities { get; set; } = "";
        public string EmploymentAchievements { get; set; } = "";
        public string Experience { get; set; } = "";
        public string DegreeName { get; set; } = "";
        public string UniversityName { get; set; } = "";
        public string PassingYear { get; set; } = "";
        public string Gpa { get; set; } = "";
        public string EducationDetails { get; set; } = "";
        public string Education { get; set; } = "";
        public string ProjectDetails { get; set; } = "";
        public string Projects { get; set; } = "";
        public string LeadershipActivityDetails { get; set; } = "";
        public string LeadershipAndActivities { get; set; } = "";
        public string CertificationDetails { get; set; } = "";
        public string Certifications { get; set; } = "";
        public string AchievementDetails { get; set; } = "";
        public string Achievements { get; set; } = "";
        public string AdditionalInformation { get; set; } = "";
        public string ReferenceDetails { get; set; } = "";
        public string Reference { get; set; } = "";
        public string TemplateKey { get; set; } = "ats-clean";
        public string PageSize { get; set; } = "A4";
    }
}
