namespace ResuniqAI.Models
{
    public class ResumeTemplateDefinition
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Accent { get; set; } = "";
        public string Description { get; set; } = "";
        public string BestFor { get; set; } = "";
        public string Tone { get; set; } = "";
        public string Variant { get; set; } = "";
        public int AtsScore { get; set; }
        public IReadOnlyList<string> Highlights { get; set; } = Array.Empty<string>();
        public string SampleName { get; set; } = "";
        public string SampleRole { get; set; } = "";
        public string SampleSummary { get; set; } = "";
    }
}
