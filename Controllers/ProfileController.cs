using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using ResuniqAI.Data;
using ResuniqAI.Models;
using ResuniqAI.ViewModels;

namespace ResuniqAI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            await EnsureProfileStorageAsync();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id)
                ?? BuildDefaultProfile(user);

            var resumes = await _context.Resumes
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var payments = await _context.Payments
                .Where(x => x.UserEmail == (user.Email ?? string.Empty))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(new ProfileDashboardViewModel
            {
                Profile = profile,
                Email = user.Email ?? string.Empty,
                IsPro = await _userManager.IsInRoleAsync(user, "Pro"),
                ResumeCount = resumes.Count,
                PremiumResumeCount = resumes.Count(x => x.TemplateKey != null && !x.TemplateKey.StartsWith("ats-", StringComparison.OrdinalIgnoreCase)),
                PaymentCount = payments.Count,
                ApprovedPaymentCount = payments.Count(x => x.IsApproved),
                Resumes = resumes.Take(8).ToList(),
                IdentityUser = user
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileDashboardViewModel model)
        {
            await EnsureProfileStorageAsync();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (profile == null)
            {
                profile = BuildDefaultProfile(user);
                _context.UserProfiles.Add(profile);
            }

            profile.UserId = user.Id;
            profile.FullName = model.Profile.FullName.Trim();
            profile.Headline = model.Profile.Headline.Trim();
            profile.Location = model.Profile.Location.Trim();
            profile.Phone = model.Profile.Phone.Trim();
            profile.LinkedIn = model.Profile.LinkedIn.Trim();
            profile.Github = model.Profile.Github.Trim();
            profile.Portfolio = model.Profile.Portfolio.Trim();
            profile.Bio = model.Profile.Bio.Trim();
            profile.UpdatedAt = DateTime.Now;

            var normalizedEmail = (model.Email ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(normalizedEmail) && !string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = normalizedEmail;
                user.UserName = normalizedEmail;
            }

            if (!string.Equals(user.PhoneNumber, profile.Phone, StringComparison.Ordinal))
                user.PhoneNumber = profile.Phone;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["ProfileSaved"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private static UserProfile BuildDefaultProfile(IdentityUser user)
        {
            return new UserProfile
            {
                UserId = user.Id,
                FullName = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty
            };
        }

        private async Task EnsureProfileStorageAsync()
        {
            await _context.Database.MigrateAsync();

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
SELECT COUNT(*)
FROM sqlite_master
WHERE type = 'table' AND name = 'UserProfiles';
""";

            var exists = Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                await _context.Database.ExecuteSqlRawAsync("""
CREATE TABLE IF NOT EXISTS "UserProfiles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_UserProfiles" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "FullName" TEXT NOT NULL,
    "Headline" TEXT NOT NULL,
    "Location" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "LinkedIn" TEXT NOT NULL,
    "Github" TEXT NOT NULL,
    "Portfolio" TEXT NOT NULL,
    "Bio" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);
""");
            }
        }
    }
}
