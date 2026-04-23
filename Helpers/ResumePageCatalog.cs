namespace ResuniqAI.Helpers
{
    public static class ResumePageCatalog
    {
        private static readonly IReadOnlyList<string> PageSizes = new[]
        {
            "A4",
            "A5",
            "A6",
            "Letter"
        };

        public static IReadOnlyList<string> GetAll() => PageSizes;

        public static string Normalize(string? pageSize)
        {
            return PageSizes.Contains(pageSize ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                ? pageSize!.ToUpperInvariant() switch
                {
                    "LETTER" => "Letter",
                    _ => pageSize!.ToUpperInvariant()
                }
                : "A4";
        }
    }
}
