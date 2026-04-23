using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResuniqAI.Data;
using ResuniqAI.Models;
using ResuniqAI.ViewModels;

namespace ResuniqAI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Revenue()
        {
            var total = _context.Payments.Count(x => x.IsApproved);
            var pending = _context.Payments.Count(x => !x.IsApproved);

            ViewBag.TotalRevenue = total * 100;
            ViewBag.Pending = pending;

            return View();
        }

        public async Task<IActionResult> ExpireSubscriptions()
        {
            var subs = await _context.Subscriptions
                .Where(x => x.IsActive && x.ExpireDate < DateTime.Now)
                .ToListAsync();

            foreach (var s in subs)
            {
                s.IsActive = false;
            }

            await _context.SaveChangesAsync();

            return Ok("Expired subscriptions updated");
        }

        public IActionResult Index()
        {
            ViewBag.TotalResumes = _context.Resumes.Count();
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.TotalPayments = _context.Payments.Count();
            ViewBag.TotalJobs = _context.JobPostings.Count();

            return View();
        }

        public IActionResult Resumes()
        {
            var data = _context.Resumes
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(data);
        }

        public IActionResult DeleteResume(int id)
        {
            var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);

            if (resume != null)
            {
                _context.Resumes.Remove(resume);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Resumes));
        }

        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult Payments()
        {
            var data = _context.Payments
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (payment == null)
                return RedirectToAction(nameof(Payments));

            payment.IsApproved = true;

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == payment.UserEmail);

            if (user != null && !await _userManager.IsInRoleAsync(user, "Pro"))
            {
                await _userManager.AddToRoleAsync(user, "Pro");
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Payments));
        }

        public async Task<IActionResult> Jobs()
        {
            var model = new AdminJobsViewModel
            {
                Jobs = await _context.JobPostings
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Jobs(AdminJobsViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewJob.Title) && !string.IsNullOrWhiteSpace(model.NewJob.CompanyName))
            {
                model.NewJob.CreatedAt = DateTime.Now;
                _context.JobPostings.Add(model.NewJob);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Jobs));
            }

            model.Jobs = await _context.JobPostings
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleJob(int id)
        {
            var job = await _context.JobPostings.FirstOrDefaultAsync(x => x.Id == id);

            if (job != null)
            {
                job.IsActive = !job.IsActive;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Jobs));
        }
    }
}
