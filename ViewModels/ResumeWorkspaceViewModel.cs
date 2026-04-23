using ResuniqAI.Models;

namespace ResuniqAI.ViewModels
{
    public class ResumeWorkspaceViewModel
    {
        public IReadOnlyList<Resume> Resumes { get; set; } = Array.Empty<Resume>();
        public IReadOnlyDictionary<string, ResumeTemplateDefinition> Templates { get; set; } =
            new Dictionary<string, ResumeTemplateDefinition>();
    }
}
