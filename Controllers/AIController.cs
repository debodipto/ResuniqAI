using Microsoft.AspNetCore.Mvc;
using ResuniqAI.Data;
using ResuniqAI.Services;

namespace ResuniqAI.Controllers
{
    public class AIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _ai;

        public AIController(ApplicationDbContext context, AIService ai)
        {
            _context = context;
            _ai = ai;
        }

        // ATS Score
        public async Task<IActionResult> ATS(int id)
        {
            var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);

            if (resume == null)
                return NotFound();

            int score = await _ai.CalculateATSScore(
                resume.Skills,
                resume.Experience,
                resume.Education
            );

            ViewBag.Score = score;

            return View(resume);
        }

        // Cover Letter
        public async Task<IActionResult> CoverLetter(int id)
        {
            var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);

            if (resume == null)
                return NotFound();

            string letter = await _ai.GenerateCoverLetter(
                resume.FullName,
                "Software Engineer"
            );

            ViewBag.Letter = letter;

            return View(resume);
        }
    }
}
