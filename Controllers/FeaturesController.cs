using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResuniqAI.Data;
using ResuniqAI.Services;
using ResuniqAI.ViewModels;
using System.Text;

namespace ResuniqAI.Controllers
{
    [Authorize]
    public class FeaturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public FeaturesController(ApplicationDbContext context, AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public IActionResult Toolkit()
        {
            return View(new CareerToolkitViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toolkit(CareerToolkitViewModel model, IFormFile? cvFile, string command)
        {
            model.ResumeText = await MergeUploadedTextAsync(model.ResumeText, cvFile);

            if (command == "cover-letter")
            {
                model.CoverLetterResult = await _aiService.GenerateCoverLetterFromResume(
                    model.CandidateName,
                    model.TargetRole,
                    model.CompanyName,
                    model.ResumeText);
            }
            else if (command == "ats-score")
            {
                var result = await _aiService.AnalyzeAtsScore(model.ResumeText, model.JobDescription);
                model.AtsScore = result.Score;
                model.AtsFeedback = result.Feedback;
            }

            return View(model);
        }

        public IActionResult InterviewCoach()
        {
            return View(new InterviewCoachViewModel
            {
                WritingPrompt = "Write a short professional email to a recruiter thanking them after an interview and confirming your interest in the role."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InterviewCoach(InterviewCoachViewModel model, IFormFile? cvFile, string command)
        {
            model.ResumeText = await MergeUploadedTextAsync(model.ResumeText, cvFile);

            if (command == "generate-questions")
            {
                model.InterviewQuestions = await _aiService.GenerateInterviewQuestions(
                    model.CandidateName,
                    model.TargetRole,
                    model.ResumeText);
            }
            else if (command == "review-answers")
            {
                if (string.IsNullOrWhiteSpace(model.InterviewQuestions))
                {
                    model.InterviewQuestions = await _aiService.GenerateInterviewQuestions(
                        model.CandidateName,
                        model.TargetRole,
                        model.ResumeText);
                }

                var result = await _aiService.EvaluateInterviewAnswers(model.ResumeText, model.InterviewAnswers);
                model.InterviewScore = result.Score;
                model.InterviewFeedback = result.Feedback;
                model.MistakesFound = result.Mistakes;
                return View("InterviewScore", model);
            }
            else if (command == "writing-exam")
            {
                var result = await _aiService.EvaluateWritingExam(model.WritingPrompt, model.WritingAnswer);
                model.WritingScore = result.Score;
                model.WritingFeedback = result.Feedback;
            }

            return View(model);
        }

        public async Task<IActionResult> JobTracker()
        {
            var model = new JobTrackerViewModel
            {
                Jobs = await _context.JobPostings
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync()
            };

            return View(model);
        }

        private static async Task<string> MergeUploadedTextAsync(string existingText, IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return existingText;

            using var memory = new MemoryStream();
            await file.CopyToAsync(memory);
            var bytes = memory.ToArray();
            var uploadedText = Encoding.UTF8.GetString(bytes).Replace("\0", " ").Trim();

            if (string.IsNullOrWhiteSpace(uploadedText))
                return existingText;

            return uploadedText;
        }
    }
}
