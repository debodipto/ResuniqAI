using ResuniqAI.Models;

namespace ResuniqAI.ViewModels
{
    public class JobTrackerViewModel
    {
        public IReadOnlyList<JobPosting> Jobs { get; set; } = Array.Empty<JobPosting>();
    }
}
