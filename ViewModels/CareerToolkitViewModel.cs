namespace ResuniqAI.ViewModels
{
    public class CareerToolkitViewModel
    {
        public string CandidateName { get; set; } = "";
        public string TargetRole { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string ResumeText { get; set; } = "";
        public string JobDescription { get; set; } = "";
        public string CoverLetterResult { get; set; } = "";
        public int AtsScore { get; set; }
        public string AtsFeedback { get; set; } = "";
    }
}
