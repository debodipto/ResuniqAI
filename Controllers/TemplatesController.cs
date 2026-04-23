using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResuniqAI.Helpers;
using ResuniqAI.ViewModels;

namespace ResuniqAI.Controllers
{
    public class TemplatesController : Controller
    {
        public IActionResult Index(int? resumeId)
        {
            var totalTemplates = ResumeTemplateCatalog.GetAll().Count;
            return View(BuildModel(
                "Resume Template Library",
                $"Choose from {totalTemplates} curated resume formats including ATS-first layouts and premium branded designs.",
                resumeId));
        }

        public IActionResult Ats(int? resumeId)
        {
            var model = BuildModel(
                "ATS Friendly Resume Formats",
                "These 6 templates are optimized for recruiter systems, easy parsing and modern hiring workflows.",
                resumeId);

            model.PremiumTemplates = Array.Empty<ResuniqAI.Models.ResumeTemplateDefinition>();
            return View(model);
        }

        public IActionResult Premium(int? resumeId)
        {
            var premiumCount = ResumeTemplateCatalog.GetPremium().Count;
            var model = BuildModel(
                "Premium Resume Formats",
                $"Explore {premiumCount} polished templates for leadership, product, design, startup and executive storytelling.",
                resumeId);

            model.AtsTemplates = Array.Empty<ResuniqAI.Models.ResumeTemplateDefinition>();
            return View(model);
        }

        private static TemplateHubViewModel BuildModel(string title, string description, int? resumeId)
        {
            return new TemplateHubViewModel
            {
                Title = title,
                Description = description,
                ResumeId = resumeId,
                CanUsePremium = false,
                AtsTemplates = ResumeTemplateCatalog.GetAtsFriendly(),
                PremiumTemplates = ResumeTemplateCatalog.GetPremium()
            };
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewData["CanUsePremium"] = User.IsInRole("Pro");
            base.OnActionExecuting(context);
        }
    }
}
