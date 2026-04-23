using Microsoft.AspNetCore.Identity;
using ResuniqAI.Models;

namespace ResuniqAI.ViewModels
{
    public class ProfileDashboardViewModel
    {
        public UserProfile Profile { get; set; } = new();
        public string Email { get; set; } = "";
        public bool IsPro { get; set; }
        public int ResumeCount { get; set; }
        public int PremiumResumeCount { get; set; }
        public int PaymentCount { get; set; }
        public int ApprovedPaymentCount { get; set; }
        public IReadOnlyList<Resume> Resumes { get; set; } = Array.Empty<Resume>();
        public IdentityUser IdentityUser { get; set; } = new();
    }
}
