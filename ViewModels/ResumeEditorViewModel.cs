using ResuniqAI.Models;

namespace ResuniqAI.ViewModels
{
    public class ResumeEditorViewModel
    {
        public Resume Resume { get; set; } = new();
        public string SelectedTemplateKey { get; set; } = "ats-clean";
        public string ActiveAiSection { get; set; } = "";
        public string AiPrompt { get; set; } = "";
        public bool IsSamplePreview { get; set; }
        public List<ResumeExperienceEntryViewModel> ExperienceEntries { get; set; } = new();
        public List<ResumeEducationEntryViewModel> EducationEntries { get; set; } = new();
        public List<ResumeProjectEntryViewModel> ProjectEntries { get; set; } = new();
        public List<ResumeLeadershipEntryViewModel> LeadershipEntries { get; set; } = new();
        public List<ResumeCertificationEntryViewModel> CertificationEntries { get; set; } = new();
        public List<ResumeAchievementEntryViewModel> AchievementEntries { get; set; } = new();
        public List<ResumeReferenceEntryViewModel> ReferenceEntries { get; set; } = new();
        public IReadOnlyList<string> PageSizes { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> InstitutionTypes { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> GradeLabels { get; set; } = Array.Empty<string>();
        public ResumeTemplateDefinition SelectedTemplate { get; set; } = new();
        public IReadOnlyList<ResumeTemplateDefinition> AtsTemplates { get; set; } = Array.Empty<ResumeTemplateDefinition>();
        public IReadOnlyList<ResumeTemplateDefinition> PremiumTemplates { get; set; } = Array.Empty<ResumeTemplateDefinition>();
    }
}
