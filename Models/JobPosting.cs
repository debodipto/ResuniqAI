namespace ResuniqAI.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string Location { get; set; } = "";
        public string EmploymentType { get; set; } = "";
        public string SalaryRange { get; set; } = "";
        public string Description { get; set; } = "";
        public string Requirements { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
