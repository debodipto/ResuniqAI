using ResuniqAI.Models;

namespace ResuniqAI.ViewModels
{
    public class AdminJobsViewModel
    {
        public JobPosting NewJob { get; set; } = new();
        public IReadOnlyList<JobPosting> Jobs { get; set; } = Array.Empty<JobPosting>();
    }
}
