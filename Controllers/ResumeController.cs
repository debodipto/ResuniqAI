using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ResuniqAI.Data;
using ResuniqAI.Helpers;
using ResuniqAI.Models;
using ResuniqAI.Services;
using ResuniqAI.ViewModels;

namespace ResuniqAI.Controllers
{
    [Authorize]
    public class ResumeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AIService _aiService;
        private readonly PdfService _pdfService;

        public ResumeController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            AIService aiService,
            PdfService pdfService)
        {
            _context = context;
            _userManager = userManager;
            _aiService = aiService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var data = await _context.Resumes
                .OrderByDescending(x => x.Id)
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            var viewModel = new ResumeWorkspaceViewModel
            {
                Resumes = data,
                Templates = ResumeTemplateCatalog
                    .GetAll()
                    .ToDictionary(x => x.Key, x => x)
            };

            return View(viewModel);
        }

        public IActionResult Create(string? template)
        {
            template = NormalizeTemplateForCurrentUser(template);

            var model = BuildEditorViewModel(
                new Resume { TemplateKey = ResumeTemplateCatalog.GetByKey(template).Key },
                template);

            return View(model);
        }

        public IActionResult SamplePreview(string? template)
        {
            var sampleResume = ResumeTemplateCatalog.CreateSampleResume(template);
            var model = BuildEditorViewModel(sampleResume, sampleResume.TemplateKey, isSamplePreview: true);

            return View(model);
        }

        public IActionResult SampleEditor(string? template)
        {
            var sampleResume = ResumeTemplateCatalog.CreateSampleResume(template);
            var model = BuildEditorViewModel(sampleResume, sampleResume.TemplateKey, isSamplePreview: true);

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResumeEditorViewModel model, string? command)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            model.SelectedTemplateKey = NormalizeTemplateForCurrentUser(model.SelectedTemplateKey);
            model.Resume.PageSize = ResumePageCatalog.Normalize(model.Resume.PageSize);
            ApplyEditorEntriesToResume(model);

            if (!string.IsNullOrWhiteSpace(command))
            {
                await ApplyAiAssistAsync(model, command);
                return View(BuildEditorViewModel(model.Resume, model.SelectedTemplateKey, model.ActiveAiSection, model.IsSamplePreview, model.AiPrompt));
            }

            if (!ModelState.IsValid)
                return View(BuildEditorViewModel(model.Resume, model.SelectedTemplateKey, model.ActiveAiSection, model.IsSamplePreview, model.AiPrompt));

            model.Resume.UserId = user.Id;
            model.Resume.TemplateKey = ResumeTemplateCatalog.GetByKey(model.SelectedTemplateKey).Key;

            _context.Resumes.Add(model.Resume);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id, string? template)
        {
            var resume = await GetUserResumeAsync(id);

            if (resume == null)
                return NotFound();

            return View(BuildEditorViewModel(resume, NormalizeTemplateForCurrentUser(template ?? resume.TemplateKey)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ResumeEditorViewModel model, string? command)
        {
            if (id != model.Resume.Id)
                return NotFound();

            var resume = await GetUserResumeAsync(id);

            if (resume == null)
                return NotFound();

            model.SelectedTemplateKey = NormalizeTemplateForCurrentUser(model.SelectedTemplateKey);
            model.Resume.PageSize = ResumePageCatalog.Normalize(model.Resume.PageSize);
            ApplyEditorEntriesToResume(model);

            if (!string.IsNullOrWhiteSpace(command))
            {
                model.Resume.UserId = resume.UserId;
                await ApplyAiAssistAsync(model, command);
                return View(BuildEditorViewModel(model.Resume, model.SelectedTemplateKey, model.ActiveAiSection, aiPrompt: model.AiPrompt));
            }

            if (!ModelState.IsValid)
            {
                model.Resume.UserId = resume.UserId;
                return View(BuildEditorViewModel(model.Resume, model.SelectedTemplateKey, model.ActiveAiSection, aiPrompt: model.AiPrompt));
            }

            resume.FullName = model.Resume.FullName;
            resume.Email = model.Resume.Email;
            resume.Phone = model.Resume.Phone;
            resume.Address = model.Resume.Address;
            resume.LinkedIn = model.Resume.LinkedIn;
            resume.Github = model.Resume.Github;
            resume.Portfolio = model.Resume.Portfolio;
            resume.Summary = model.Resume.Summary;
            resume.Skills = model.Resume.Skills;
            resume.CompanyName = model.Resume.CompanyName;
            resume.PositionTitle = model.Resume.PositionTitle;
            resume.EmploymentDuration = model.Resume.EmploymentDuration;
            resume.EmploymentLocation = model.Resume.EmploymentLocation;
            resume.EmploymentResponsibilities = model.Resume.EmploymentResponsibilities;
            resume.EmploymentAchievements = model.Resume.EmploymentAchievements;
            resume.Experience = model.Resume.Experience;
            resume.DegreeName = model.Resume.DegreeName;
            resume.UniversityName = model.Resume.UniversityName;
            resume.PassingYear = model.Resume.PassingYear;
            resume.Gpa = model.Resume.Gpa;
            resume.EducationDetails = model.Resume.EducationDetails;
            resume.Education = model.Resume.Education;
            resume.ProjectDetails = model.Resume.ProjectDetails;
            resume.Projects = model.Resume.Projects;
            resume.LeadershipActivityDetails = model.Resume.LeadershipActivityDetails;
            resume.LeadershipAndActivities = model.Resume.LeadershipAndActivities;
            resume.CertificationDetails = model.Resume.CertificationDetails;
            resume.Certifications = model.Resume.Certifications;
            resume.AchievementDetails = model.Resume.AchievementDetails;
            resume.Achievements = model.Resume.Achievements;
            resume.AdditionalInformation = model.Resume.AdditionalInformation;
            resume.ReferenceDetails = model.Resume.ReferenceDetails;
            resume.Reference = model.Resume.Reference;
            resume.TemplateKey = ResumeTemplateCatalog.GetByKey(model.SelectedTemplateKey).Key;
            resume.PageSize = ResumePageCatalog.Normalize(model.Resume.PageSize);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GenerateAI(int id)
        {
            var resume = await GetUserResumeAsync(id);

            if (resume == null)
                return NotFound();

            NormalizeResume(resume);
            resume.Summary = await _aiService.GenerateSummary(resume.Skills, resume.Experience);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var resume = await GetUserResumeAsync(id);

            if (resume == null)
                return NotFound();

            var pdfBytes = _pdfService.GenerateResumePdf(resume);
            var fileName = string.IsNullOrWhiteSpace(resume.FullName)
                ? "resume.pdf"
                : $"{resume.FullName.Replace(' ', '_')}_Resume.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreviewPdf(ResumeEditorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            model.SelectedTemplateKey = NormalizeTemplateForCurrentUser(model.SelectedTemplateKey);
            model.Resume.PageSize = ResumePageCatalog.Normalize(model.Resume.PageSize);
            ApplyEditorEntriesToResume(model);
            model.Resume.TemplateKey = ResumeTemplateCatalog.GetByKey(model.SelectedTemplateKey).Key;

            var pdfBytes = _pdfService.GenerateResumePdf(model.Resume);
            var fileName = string.IsNullOrWhiteSpace(model.Resume.FullName)
                ? "resume-preview.pdf"
                : $"{model.Resume.FullName.Replace(' ', '_')}_Preview.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        private async Task<Resume?> GetUserResumeAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return null;

            return await _context.Resumes
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);
        }

        private async Task ApplyAiAssistAsync(ResumeEditorViewModel model, string command)
        {
            switch (command)
            {
                case "ai-summary":
                    model.Resume.Summary = await _aiService.GenerateSummaryFromPrompt(
                        model.AiPrompt,
                        model.Resume.Skills,
                        model.Resume.Experience,
                        model.Resume.PositionTitle,
                        model.Resume.CompanyName);
                    model.ActiveAiSection = "summary";
                    break;
                case "ai-experience":
                    EnsureEntrySlots(model);
                    var experienceIndex = GetTargetExperienceIndex(model.ExperienceEntries);
                    var experienceEntry = model.ExperienceEntries[experienceIndex];
                    experienceEntry.Responsibilities = await _aiService.GenerateExperienceBulletsFromPrompt(
                        model.AiPrompt,
                        experienceEntry.CompanyName,
                        experienceEntry.PositionTitle,
                        experienceEntry.EmploymentDuration,
                        model.Resume.Skills);
                    experienceEntry.Achievements = await _aiService.GenerateAchievementHighlightsFromPrompt(
                        model.AiPrompt,
                        experienceEntry.PositionTitle,
                        model.Resume.Skills);
                    ApplyEditorEntriesToResume(model);
                    model.ActiveAiSection = "experience";
                    break;
                case "ai-education":
                    EnsureEntrySlots(model);
                    var educationIndex = GetTargetEducationIndex(model.EducationEntries);
                    var educationEntry = model.EducationEntries[educationIndex];
                    educationEntry.Details = await _aiService.GenerateEducationDetailsFromPrompt(
                        model.AiPrompt,
                        educationEntry.DegreeName,
                        educationEntry.UniversityName,
                        educationEntry.PassingYear,
                        educationEntry.Gpa);
                    ApplyEditorEntriesToResume(model);
                    model.ActiveAiSection = "education";
                    break;
            }
        }

        private static void NormalizeResume(Resume resume)
        {
            var experienceEntries = ParseExperienceEntries(resume);
            var educationEntries = ParseEducationEntries(resume);
            var projectEntries = ParseProjectEntries(resume);
            var leadershipEntries = ParseLeadershipEntries(resume);
            var certificationEntries = ParseCertificationEntries(resume);
            var achievementEntries = ParseAchievementEntries(resume);
            var referenceEntries = ParseReferenceEntries(resume);

            resume.Experience = BuildExperienceText(experienceEntries);
            resume.Education = BuildEducationText(educationEntries);
            resume.Projects = BuildProjectsText(projectEntries);
            resume.LeadershipAndActivities = BuildLeadershipText(leadershipEntries);
            resume.Certifications = BuildCertificationsText(certificationEntries);
            resume.Achievements = BuildAchievementsText(achievementEntries);
            resume.Reference = BuildReferenceText(referenceEntries);
        }

        private static ResumeEditorViewModel BuildEditorViewModel(Resume resume, string? templateKey, string activeAiSection = "", bool isSamplePreview = false, string aiPrompt = "")
        {
            NormalizeResume(resume);

            var normalizedTemplate = ResumeTemplateCatalog.GetByKey(templateKey ?? resume.TemplateKey);
            resume.TemplateKey = normalizedTemplate.Key;
            resume.PageSize = ResumePageCatalog.Normalize(resume.PageSize);
            var experienceEntries = ParseExperienceEntries(resume);
            var educationEntries = ParseEducationEntries(resume);
            var projectEntries = ParseProjectEntries(resume);
            var leadershipEntries = ParseLeadershipEntries(resume);
            var certificationEntries = ParseCertificationEntries(resume);
            var achievementEntries = ParseAchievementEntries(resume);
            var referenceEntries = ParseReferenceEntries(resume);
            EnsureEntrySlots(experienceEntries, 4, () => new ResumeExperienceEntryViewModel());
            EnsureEntrySlots(educationEntries, 4, () => new ResumeEducationEntryViewModel());
            EnsureEntrySlots(projectEntries, 4, () => new ResumeProjectEntryViewModel());
            EnsureEntrySlots(leadershipEntries, 4, () => new ResumeLeadershipEntryViewModel());
            EnsureEntrySlots(certificationEntries, 4, () => new ResumeCertificationEntryViewModel());
            EnsureEntrySlots(achievementEntries, 4, () => new ResumeAchievementEntryViewModel());
            EnsureEntrySlots(referenceEntries, 3, () => new ResumeReferenceEntryViewModel());
            foreach (var entry in educationEntries)
            {
                entry.InstitutionType = string.IsNullOrWhiteSpace(entry.InstitutionType) ? "University" : entry.InstitutionType;
                entry.GradeLabel = string.IsNullOrWhiteSpace(entry.GradeLabel) ? "CGPA" : entry.GradeLabel;
            }

            return new ResumeEditorViewModel
            {
                Resume = resume,
                SelectedTemplateKey = normalizedTemplate.Key,
                ActiveAiSection = activeAiSection,
                AiPrompt = aiPrompt,
                IsSamplePreview = isSamplePreview,
                ExperienceEntries = experienceEntries,
                EducationEntries = educationEntries,
                ProjectEntries = projectEntries,
                LeadershipEntries = leadershipEntries,
                CertificationEntries = certificationEntries,
                AchievementEntries = achievementEntries,
                ReferenceEntries = referenceEntries,
                PageSizes = ResumePageCatalog.GetAll(),
                InstitutionTypes = new[] { "University", "College", "School" },
                GradeLabels = new[] { "CGPA", "GPA" },
                SelectedTemplate = normalizedTemplate,
                AtsTemplates = ResumeTemplateCatalog.GetAtsFriendly(),
                PremiumTemplates = ResumeTemplateCatalog.GetPremium()
            };
        }

        private static void ApplyEditorEntriesToResume(ResumeEditorViewModel model)
        {
            EnsureEntrySlots(model);

            var experienceEntries = model.ExperienceEntries
                .Where(IsMeaningfulExperienceEntry)
                .ToList();

            var educationEntries = model.EducationEntries
                .Where(IsMeaningfulEducationEntry)
                .ToList();
            var projectEntries = model.ProjectEntries
                .Where(IsMeaningfulProjectEntry)
                .ToList();
            var leadershipEntries = model.LeadershipEntries
                .Where(IsMeaningfulLeadershipEntry)
                .ToList();
            var certificationEntries = model.CertificationEntries
                .Where(IsMeaningfulCertificationEntry)
                .ToList();
            var achievementEntries = model.AchievementEntries
                .Where(IsMeaningfulAchievementEntry)
                .ToList();
            var referenceEntries = model.ReferenceEntries
                .Where(IsMeaningfulReferenceEntry)
                .ToList();

            var primaryExperience = experienceEntries.FirstOrDefault() ?? new ResumeExperienceEntryViewModel();
            var primaryEducation = educationEntries.FirstOrDefault() ?? new ResumeEducationEntryViewModel();

            model.Resume.CompanyName = primaryExperience.CompanyName;
            model.Resume.PositionTitle = primaryExperience.PositionTitle;
            model.Resume.EmploymentDuration = primaryExperience.EmploymentDuration;
            model.Resume.EmploymentLocation = primaryExperience.EmploymentLocation;
            model.Resume.EmploymentResponsibilities = JsonSerializer.Serialize(experienceEntries);
            model.Resume.EmploymentAchievements = primaryExperience.Achievements;

            model.Resume.DegreeName = primaryEducation.DegreeName;
            model.Resume.UniversityName = primaryEducation.UniversityName;
            model.Resume.PassingYear = primaryEducation.PassingYear;
            model.Resume.Gpa = primaryEducation.Gpa;
            model.Resume.EducationDetails = JsonSerializer.Serialize(educationEntries);

            model.Resume.Experience = BuildExperienceText(experienceEntries);
            model.Resume.Education = BuildEducationText(educationEntries);
            model.Resume.ProjectDetails = JsonSerializer.Serialize(projectEntries);
            model.Resume.Projects = BuildProjectsText(projectEntries);
            model.Resume.LeadershipActivityDetails = JsonSerializer.Serialize(leadershipEntries);
            model.Resume.LeadershipAndActivities = BuildLeadershipText(leadershipEntries);
            model.Resume.CertificationDetails = JsonSerializer.Serialize(certificationEntries);
            model.Resume.Certifications = BuildCertificationsText(certificationEntries);
            model.Resume.AchievementDetails = JsonSerializer.Serialize(achievementEntries);
            model.Resume.Achievements = BuildAchievementsText(achievementEntries);
            model.Resume.ReferenceDetails = JsonSerializer.Serialize(referenceEntries);
            model.Resume.Reference = BuildReferenceText(referenceEntries);
        }

        private static List<ResumeExperienceEntryViewModel> ParseExperienceEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeExperienceEntryViewModel>(resume.EmploymentResponsibilities);
            if (parsed.Any())
                return parsed;

            return new List<ResumeExperienceEntryViewModel>
            {
                new()
                {
                    CompanyName = resume.CompanyName,
                    PositionTitle = resume.PositionTitle,
                    EmploymentDuration = resume.EmploymentDuration,
                    EmploymentLocation = resume.EmploymentLocation,
                    Responsibilities = resume.EmploymentResponsibilities,
                    Achievements = resume.EmploymentAchievements
                }
            }.Where(IsMeaningfulExperienceEntry).ToList();
        }

        private static List<ResumeEducationEntryViewModel> ParseEducationEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeEducationEntryViewModel>(resume.EducationDetails);
            if (parsed.Any())
                return parsed;

            return new List<ResumeEducationEntryViewModel>
            {
                new()
                {
                    DegreeName = resume.DegreeName,
                    InstitutionType = "University",
                    UniversityName = resume.UniversityName,
                    PassingYear = resume.PassingYear,
                    GradeLabel = "CGPA",
                    Gpa = resume.Gpa,
                    Details = resume.EducationDetails
                }
            }.Where(IsMeaningfulEducationEntry).ToList();
        }

        private static List<ResumeProjectEntryViewModel> ParseProjectEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeProjectEntryViewModel>(resume.ProjectDetails);
            if (parsed.Any())
                return parsed;

            return new List<ResumeProjectEntryViewModel>().Where(IsMeaningfulProjectEntry).ToList();
        }

        private static List<ResumeLeadershipEntryViewModel> ParseLeadershipEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeLeadershipEntryViewModel>(resume.LeadershipActivityDetails);
            if (parsed.Any())
                return parsed;

            return new List<ResumeLeadershipEntryViewModel>().Where(IsMeaningfulLeadershipEntry).ToList();
        }

        private static List<ResumeCertificationEntryViewModel> ParseCertificationEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeCertificationEntryViewModel>(resume.CertificationDetails);
            if (parsed.Any())
                return parsed;

            return new List<ResumeCertificationEntryViewModel>().Where(IsMeaningfulCertificationEntry).ToList();
        }

        private static List<ResumeAchievementEntryViewModel> ParseAchievementEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeAchievementEntryViewModel>(resume.AchievementDetails);
            if (parsed.Any())
                return parsed;

            return new List<ResumeAchievementEntryViewModel>().Where(IsMeaningfulAchievementEntry).ToList();
        }

        private static List<ResumeReferenceEntryViewModel> ParseReferenceEntries(Resume resume)
        {
            var parsed = TryDeserialize<ResumeReferenceEntryViewModel>(resume.ReferenceDetails);
            if (parsed.Any())
                return parsed;

            return new List<ResumeReferenceEntryViewModel>().Where(IsMeaningfulReferenceEntry).ToList();
        }

        private static List<T> TryDeserialize<T>(string payload) where T : class
        {
            if (string.IsNullOrWhiteSpace(payload))
                return new List<T>();

            try
            {
                var result = JsonSerializer.Deserialize<List<T>>(payload);
                return result ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private static string BuildExperienceText(IEnumerable<ResumeExperienceEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulExperienceEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();
                    var headlineParts = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.PositionTitle))
                        headlineParts.Add(entry.PositionTitle);

                    if (!string.IsNullOrWhiteSpace(entry.CompanyName))
                        headlineParts.Add(string.IsNullOrWhiteSpace(entry.PositionTitle)
                            ? entry.CompanyName
                            : $"at {entry.CompanyName}");

                    if (headlineParts.Any())
                        lines.Add(string.Join(" ", headlineParts));

                    if (!string.IsNullOrWhiteSpace(entry.EmploymentDuration))
                        lines.Add($"Duration: {entry.EmploymentDuration}");

                    if (!string.IsNullOrWhiteSpace(entry.EmploymentLocation))
                        lines.Add($"Location: {entry.EmploymentLocation}");

                    if (!string.IsNullOrWhiteSpace(entry.Responsibilities))
                        lines.Add($"Responsibilities:\n{entry.Responsibilities}");

                    if (!string.IsNullOrWhiteSpace(entry.Achievements))
                        lines.Add($"Achievements:\n{entry.Achievements}");

                    return string.Join("\n\n", lines);
                }));
        }

        private static string BuildEducationText(IEnumerable<ResumeEducationEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulEducationEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.DegreeName))
                        lines.Add($"Degree: {entry.DegreeName}");

                    if (!string.IsNullOrWhiteSpace(entry.UniversityName))
                    {
                        var institutionType = string.IsNullOrWhiteSpace(entry.InstitutionType)
                            ? "University"
                            : entry.InstitutionType;
                        lines.Add($"{institutionType}: {entry.UniversityName}");
                    }

                    if (!string.IsNullOrWhiteSpace(entry.PassingYear))
                        lines.Add($"Passing Year: {entry.PassingYear}");

                    if (!string.IsNullOrWhiteSpace(entry.Gpa))
                    {
                        var gradeLabel = string.IsNullOrWhiteSpace(entry.GradeLabel)
                            ? "CGPA"
                            : entry.GradeLabel;
                        lines.Add($"{gradeLabel}: {entry.Gpa}");
                    }

                    if (!string.IsNullOrWhiteSpace(entry.Details))
                        lines.Add($"Details: {entry.Details}");

                    return string.Join("\n", lines);
                }));
        }

        private static string BuildProjectsText(IEnumerable<ResumeProjectEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulProjectEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();
                    var headlineParts = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.ProjectName))
                        headlineParts.Add(entry.ProjectName);

                    if (!string.IsNullOrWhiteSpace(entry.RoleName))
                        headlineParts.Add($"Role: {entry.RoleName}");

                    if (headlineParts.Any())
                        lines.Add(string.Join(" | ", headlineParts));

                    if (!string.IsNullOrWhiteSpace(entry.Duration))
                        lines.Add($"Duration: {entry.Duration}");

                    if (!string.IsNullOrWhiteSpace(entry.TechStack))
                        lines.Add($"Tech Stack: {entry.TechStack}");

                    if (!string.IsNullOrWhiteSpace(entry.ProjectLink))
                        lines.Add($"Link: {entry.ProjectLink}");

                    if (!string.IsNullOrWhiteSpace(entry.Details))
                        lines.Add($"Details:\n{entry.Details}");

                    return string.Join("\n", lines);
                }));
        }

        private static string BuildLeadershipText(IEnumerable<ResumeLeadershipEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulLeadershipEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.RoleTitle) || !string.IsNullOrWhiteSpace(entry.OrganizationName))
                    {
                        var title = entry.RoleTitle;
                        var organization = entry.OrganizationName;
                        lines.Add(string.IsNullOrWhiteSpace(title) ? organization : $"{title} at {organization}");
                    }

                    if (!string.IsNullOrWhiteSpace(entry.Duration))
                        lines.Add($"Duration: {entry.Duration}");

                    if (!string.IsNullOrWhiteSpace(entry.Details))
                        lines.Add($"Details:\n{entry.Details}");

                    return string.Join("\n", lines);
                }));
        }

        private static string BuildCertificationsText(IEnumerable<ResumeCertificationEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulCertificationEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.CertificationName))
                        lines.Add($"Certification: {entry.CertificationName}");

                    if (!string.IsNullOrWhiteSpace(entry.Issuer))
                        lines.Add($"Issuer: {entry.Issuer}");

                    if (!string.IsNullOrWhiteSpace(entry.Year))
                        lines.Add($"Year: {entry.Year}");

                    if (!string.IsNullOrWhiteSpace(entry.CredentialId))
                        lines.Add($"Credential ID: {entry.CredentialId}");

                    if (!string.IsNullOrWhiteSpace(entry.Details))
                        lines.Add($"Details: {entry.Details}");

                    return string.Join("\n", lines);
                }));
        }

        private static string BuildAchievementsText(IEnumerable<ResumeAchievementEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulAchievementEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.Title))
                        lines.Add($"Achievement: {entry.Title}");

                    if (!string.IsNullOrWhiteSpace(entry.Issuer))
                        lines.Add($"Issuer: {entry.Issuer}");

                    if (!string.IsNullOrWhiteSpace(entry.Year))
                        lines.Add($"Year: {entry.Year}");

                    if (!string.IsNullOrWhiteSpace(entry.Details))
                        lines.Add($"Details: {entry.Details}");

                    return string.Join("\n", lines);
                }));
        }

        private static string BuildReferenceText(IEnumerable<ResumeReferenceEntryViewModel> entries)
        {
            return string.Join("\n\n", entries
                .Where(IsMeaningfulReferenceEntry)
                .Select(entry =>
                {
                    var lines = new List<string>();

                    if (!string.IsNullOrWhiteSpace(entry.ReferenceName))
                        lines.Add(entry.ReferenceName);

                    if (!string.IsNullOrWhiteSpace(entry.PositionTitle))
                        lines.Add($"Position: {entry.PositionTitle}");

                    if (!string.IsNullOrWhiteSpace(entry.OrganizationName))
                        lines.Add($"Organization: {entry.OrganizationName}");

                    if (!string.IsNullOrWhiteSpace(entry.Email))
                        lines.Add($"Email: {entry.Email}");

                    if (!string.IsNullOrWhiteSpace(entry.Phone))
                        lines.Add($"Phone: {entry.Phone}");

                    if (!string.IsNullOrWhiteSpace(entry.Details))
                        lines.Add($"Details: {entry.Details}");

                    return string.Join("\n", lines);
                }));
        }

        private static bool IsMeaningfulExperienceEntry(ResumeExperienceEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.CompanyName)
                || !string.IsNullOrWhiteSpace(entry.PositionTitle)
                || !string.IsNullOrWhiteSpace(entry.EmploymentDuration)
                || !string.IsNullOrWhiteSpace(entry.EmploymentLocation)
                || !string.IsNullOrWhiteSpace(entry.Responsibilities)
                || !string.IsNullOrWhiteSpace(entry.Achievements);
        }

        private static bool IsMeaningfulEducationEntry(ResumeEducationEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.DegreeName)
                || !string.IsNullOrWhiteSpace(entry.InstitutionType)
                || !string.IsNullOrWhiteSpace(entry.UniversityName)
                || !string.IsNullOrWhiteSpace(entry.PassingYear)
                || !string.IsNullOrWhiteSpace(entry.GradeLabel)
                || !string.IsNullOrWhiteSpace(entry.Gpa)
                || !string.IsNullOrWhiteSpace(entry.Details);
        }

        private static bool IsMeaningfulProjectEntry(ResumeProjectEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.ProjectName)
                || !string.IsNullOrWhiteSpace(entry.RoleName)
                || !string.IsNullOrWhiteSpace(entry.Duration)
                || !string.IsNullOrWhiteSpace(entry.TechStack)
                || !string.IsNullOrWhiteSpace(entry.ProjectLink)
                || !string.IsNullOrWhiteSpace(entry.Details);
        }

        private static bool IsMeaningfulLeadershipEntry(ResumeLeadershipEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.OrganizationName)
                || !string.IsNullOrWhiteSpace(entry.RoleTitle)
                || !string.IsNullOrWhiteSpace(entry.Duration)
                || !string.IsNullOrWhiteSpace(entry.Details);
        }

        private static bool IsMeaningfulCertificationEntry(ResumeCertificationEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.CertificationName)
                || !string.IsNullOrWhiteSpace(entry.Issuer)
                || !string.IsNullOrWhiteSpace(entry.Year)
                || !string.IsNullOrWhiteSpace(entry.CredentialId)
                || !string.IsNullOrWhiteSpace(entry.Details);
        }

        private static bool IsMeaningfulAchievementEntry(ResumeAchievementEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.Title)
                || !string.IsNullOrWhiteSpace(entry.Issuer)
                || !string.IsNullOrWhiteSpace(entry.Year)
                || !string.IsNullOrWhiteSpace(entry.Details);
        }

        private static bool IsMeaningfulReferenceEntry(ResumeReferenceEntryViewModel entry)
        {
            return !string.IsNullOrWhiteSpace(entry.ReferenceName)
                || !string.IsNullOrWhiteSpace(entry.PositionTitle)
                || !string.IsNullOrWhiteSpace(entry.OrganizationName)
                || !string.IsNullOrWhiteSpace(entry.Email)
                || !string.IsNullOrWhiteSpace(entry.Phone)
                || !string.IsNullOrWhiteSpace(entry.Details);
        }

        private static void EnsureEntrySlots(ResumeEditorViewModel model)
        {
            EnsureEntrySlots(model.ExperienceEntries, 4, () => new ResumeExperienceEntryViewModel());
            EnsureEntrySlots(model.EducationEntries, 4, () => new ResumeEducationEntryViewModel());
            EnsureEntrySlots(model.ProjectEntries, 4, () => new ResumeProjectEntryViewModel());
            EnsureEntrySlots(model.LeadershipEntries, 4, () => new ResumeLeadershipEntryViewModel());
            EnsureEntrySlots(model.CertificationEntries, 4, () => new ResumeCertificationEntryViewModel());
            EnsureEntrySlots(model.AchievementEntries, 4, () => new ResumeAchievementEntryViewModel());
            EnsureEntrySlots(model.ReferenceEntries, 3, () => new ResumeReferenceEntryViewModel());
        }

        private static void EnsureEntrySlots<T>(List<T> entries, int count, Func<T> factory)
        {
            while (entries.Count < count)
                entries.Add(factory());
        }

        private static int GetTargetExperienceIndex(IReadOnlyList<ResumeExperienceEntryViewModel> entries)
        {
            for (var i = 0; i < entries.Count; i++)
            {
                if (IsMeaningfulExperienceEntry(entries[i]))
                    return i;
            }

            return 0;
        }

        private static int GetTargetEducationIndex(IReadOnlyList<ResumeEducationEntryViewModel> entries)
        {
            for (var i = 0; i < entries.Count; i++)
            {
                if (IsMeaningfulEducationEntry(entries[i]))
                    return i;
            }

            return 0;
        }

        private string NormalizeTemplateForCurrentUser(string? templateKey)
        {
            var template = ResumeTemplateCatalog.GetByKey(templateKey);

            if (template.Category == "Premium Formats" && !User.IsInRole("Pro"))
                return "ats-clean";

            return template.Key;
        }
    }
}
