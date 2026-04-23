using ResuniqAI.Models;

namespace ResuniqAI.ViewModels
{
    public class TemplateHubViewModel
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int? ResumeId { get; set; }
        public bool CanUsePremium { get; set; }
        public string PrimaryCta { get; set; } = "Use Template";
        public IReadOnlyList<ResumeTemplateDefinition> AtsTemplates { get; set; } = Array.Empty<ResumeTemplateDefinition>();
        public IReadOnlyList<ResumeTemplateDefinition> PremiumTemplates { get; set; } = Array.Empty<ResumeTemplateDefinition>();
    }
}
