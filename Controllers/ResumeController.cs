using Microsoft.AspNetCore.Mvc;
using ResuniqAI.Data;
using ResuniqAI.Models;
using ResuniqAI.Services;

namespace ResuniqAI.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _ai;
        private readonly PdfService _pdf;

        public ResumeController(
            ApplicationDbContext context,
            AIService ai,
            PdfService pdf)
        {
            _context = context;
            _ai = ai;
            _pdf = pdf;
        }

        // 📄 LIST PAGE
        public IActionResult Index()
        {
            var data = _context.Resumes.ToList();
            return View(data);
        }

        // ➕ CREATE PAGE
        public IActionResult Create()
        {
            return View();
        }

        // 💾 SAVE RESUME
        [HttpPost]
        public IActionResult Create(Resume resume)
        {
            if (ModelState.IsValid)
            {
                _context.Resumes.Add(resume);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(resume);
        }

        // 🤖 AI GENERATE SUMMARY
        public async Task<IActionResult> GenerateAI(int id)
        {
            var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);

            if (resume != null)
            {
                resume.Summary = await _ai.GenerateSummary(resume.Skills, resume.Experience);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // 📄 DOWNLOAD PDF
        public IActionResult DownloadPdf(int id)
        {
            var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);

            if (resume == null)
                return NotFound();

            var pdfBytes = _pdf.GenerateResumePdf(resume);

            return File(pdfBytes, "application/pdf", $"{resume.FullName}_Resume.pdf");
        }
    }
}