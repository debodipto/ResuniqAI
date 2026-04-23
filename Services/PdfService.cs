using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResuniqAI.Helpers;
using ResuniqAI.Models;
using System.Text.RegularExpressions;

namespace ResuniqAI.Services
{
    public class PdfService
    {
        public byte[] GenerateResumePdf(Resume r)
        {
            var template = ResumeTemplateCatalog.GetByKey(r.TemplateKey);
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(GetPageSize(r.PageSize));
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Element(content =>
                    {
                        switch (template.Variant)
                        {
                            case "executive":
                                BuildExecutive(content, r, template);
                                break;
                            case "split":
                                BuildSplit(content, r, template);
                                break;
                            case "timeline":
                                BuildTimeline(content, r, template);
                                break;
                            default:
                                BuildMinimal(content, r, template);
                                break;
                        }
                    });
                });
            });

            return doc.GeneratePdf();
        }

        private static void BuildMinimal(IContainer container, Resume resume, ResumeTemplateDefinition template)
        {
            container.Column(col =>
            {
                col.Item().Text(resume.FullName).FontSize(24).Bold().FontColor(template.Accent);
                BuildContactLine(col.Item(), resume, template.Accent);
                AddCommonSections(col, resume, template, includeProjectsWithLinks: false);
            });
        }

        private static void BuildExecutive(IContainer container, Resume resume, ResumeTemplateDefinition template)
        {
            container.Column(col =>
            {
                col.Item().Background(template.Accent).Padding(16).Column(header =>
                {
                    header.Item().Text(resume.FullName).FontColor("#FFFFFF").FontSize(26).Bold();
                    BuildContactLine(header.Item(), resume, "#F8FAFC");
                });

                AddCommonSections(col, resume, template, includeProjectsWithLinks: false, summaryTitle: "Executive Summary", skillsTitle: "Core Skills", experienceTitle: "Professional Experience");
            });
        }

        private static void BuildSplit(IContainer container, Resume resume, ResumeTemplateDefinition template)
        {
            container.Row(row =>
            {
                row.RelativeItem(1).Background("#F8FAFC").Padding(16).Column(sidebar =>
                {
                    sidebar.Item().Text(resume.FullName).FontSize(22).Bold().FontColor(template.Accent);
                    BuildContactStack(sidebar, resume, "#243447");
                    sidebar.Item().PaddingTop(12).Element(x => BuildSection(x, "Skills", resume.Skills, template.Accent));
                    sidebar.Item().Element(x => BuildSection(x, "Education", resume.Education, template.Accent));
                });

                row.RelativeItem(2).PaddingLeft(16).Column(main =>
                {
                    main.Item().Element(x => BuildSection(x, "Profile", resume.Summary, template.Accent));
                    main.Item().Element(x => BuildSection(x, "Experience", resume.Experience, template.Accent));
                    if (!string.IsNullOrWhiteSpace(resume.Projects) || !string.IsNullOrWhiteSpace(resume.Portfolio))
                    {
                        var projectContent = string.IsNullOrWhiteSpace(resume.Portfolio)
                            ? resume.Projects
                            : string.IsNullOrWhiteSpace(resume.Projects)
                                ? resume.Portfolio
                                : $"{resume.Projects}\n\nPortfolio: {resume.Portfolio}";

                        main.Item().Element(x => BuildSection(x, "Projects & Links", projectContent, template.Accent));
                    }

                    AppendSupportingSections(main, resume, template.Accent);
                });
            });
        }

        private static void BuildTimeline(IContainer container, Resume resume, ResumeTemplateDefinition template)
        {
            container.Column(col =>
            {
                col.Item().Text(resume.FullName).FontSize(24).Bold();
                BuildContactLine(col.Item(), resume, template.Accent);
                col.Item().PaddingVertical(12).LineHorizontal(1).LineColor(template.Accent);
                AddCommonSections(col, resume, template, includeProjectsWithLinks: true, summaryTitle: "Career Snapshot", skillsTitle: "Skill Stack", experienceTitle: "Experience Timeline");
            });
        }

        private static void BuildContactLine(IContainer container, Resume resume, string color)
        {
            var contactItems = GetContactItems(resume);

            if (contactItems.Count == 0)
                return;

            container.Text(string.Join(" | ", contactItems)).FontColor(color);
        }

        private static void BuildContactStack(ColumnDescriptor column, Resume resume, string color)
        {
            foreach (var item in GetContactItems(resume))
            {
                column.Item().PaddingTop(8).Text(item).FontColor(color);
            }
        }

        private static List<string> GetContactItems(Resume resume)
        {
            var items = new List<string>();

            AddIfFilled(items, resume.Email);
            AddIfFilled(items, resume.Phone);
            AddIfFilled(items, resume.Address);
            AddIfFilled(items, resume.LinkedIn);
            AddIfFilled(items, resume.Github);
            AddIfFilled(items, resume.Portfolio);

            return items;
        }

        private static void AddIfFilled(List<string> items, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                items.Add(value.Trim());
        }

        private static void AddCommonSections(ColumnDescriptor col, Resume resume, ResumeTemplateDefinition template, bool includeProjectsWithLinks, string summaryTitle = "Summary", string skillsTitle = "Skills", string experienceTitle = "Experience")
        {
            col.Item().PaddingTop(12).Element(x => BuildSection(x, summaryTitle, resume.Summary, template.Accent));
            col.Item().Element(x => BuildSection(x, skillsTitle, resume.Skills, template.Accent));
            col.Item().Element(x => BuildSection(x, experienceTitle, resume.Experience, template.Accent));
            col.Item().Element(x => BuildSection(x, "Education", resume.Education, template.Accent));

            if (!string.IsNullOrWhiteSpace(resume.Projects))
            {
                var projectContent = includeProjectsWithLinks && !string.IsNullOrWhiteSpace(resume.Portfolio)
                    ? $"{resume.Projects}\n\nPortfolio: {resume.Portfolio}"
                    : resume.Projects;

                col.Item().Element(x => BuildSection(x, "Projects", projectContent, template.Accent));
            }

            AppendSupportingSections(col, resume, template.Accent);
        }

        private static void AppendSupportingSections(ColumnDescriptor col, Resume resume, string accent)
        {
            AppendOptionalSection(col, "Leadership & Activities", resume.LeadershipAndActivities, accent);
            AppendOptionalSection(col, "Certifications", resume.Certifications, accent);
            AppendOptionalSection(col, "Achievements", resume.Achievements, accent);
            AppendOptionalSection(col, "Additional Information", resume.AdditionalInformation, accent);
            AppendOptionalSection(col, "Reference", resume.Reference, accent);
        }

        private static void AppendOptionalSection(ColumnDescriptor col, string title, string content, string accent)
        {
            if (!string.IsNullOrWhiteSpace(content))
                col.Item().Element(x => BuildSection(x, title, content, accent));
        }

        private static void BuildSection(IContainer container, string title, string content, string accent)
        {
            container.PaddingBottom(10).Column(col =>
            {
                col.Item().Text(title).Bold().FontColor(accent);
                if (string.IsNullOrWhiteSpace(content))
                {
                    col.Item().PaddingTop(4).Text("Add content in the editor to complete this section.");
                }
                else
                {
                    col.Item().PaddingTop(4).Element(x => BuildSectionContent(x, title, content));
                }
            });
        }

        private static void BuildSectionContent(IContainer container, string title, string content)
        {
            var normalizedContent = Regex.Replace(content, @"(\r?\n\s*){2,}", "\n\n").Trim();
            var paragraphs = Regex
                .Split(normalizedContent, @"\r?\n\r?\n")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            container.Column(col =>
            {
                foreach (var paragraph in paragraphs)
                {
                    var lines = paragraph
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if (lines.Length == 0)
                        continue;

                    col.Item().PaddingBottom(4).Column(lineCol =>
                    {
                        for (var i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            var isBold = ShouldBoldLine(title, line, i);

                            if (isBold)
                                lineCol.Item().Text(line).Bold();
                            else
                                lineCol.Item().Text(line);
                        }
                    });
                }
            });
        }

        private static bool ShouldBoldLine(string sectionTitle, string line, int lineIndex)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            if (sectionTitle.Contains("Experience", StringComparison.OrdinalIgnoreCase))
                return lineIndex == 0;

            if (sectionTitle.Contains("Projects", StringComparison.OrdinalIgnoreCase))
                return lineIndex == 0;

            if (sectionTitle.Contains("Education", StringComparison.OrdinalIgnoreCase))
                return line.StartsWith("Degree:", StringComparison.OrdinalIgnoreCase);

            if (sectionTitle.Contains("Certifications", StringComparison.OrdinalIgnoreCase))
                return line.StartsWith("Certification:", StringComparison.OrdinalIgnoreCase);

            if (sectionTitle.Contains("Achievements", StringComparison.OrdinalIgnoreCase))
                return line.StartsWith("Achievement:", StringComparison.OrdinalIgnoreCase);

            if (sectionTitle.Contains("Reference", StringComparison.OrdinalIgnoreCase))
                return lineIndex == 0;

            return false;
        }

        private static PageSize GetPageSize(string? pageSize)
        {
            return ResumePageCatalog.Normalize(pageSize) switch
            {
                "A5" => PageSizes.A5,
                "A6" => PageSizes.A6,
                "Letter" => PageSizes.Letter,
                _ => PageSizes.A4
            };
        }
    }
}
