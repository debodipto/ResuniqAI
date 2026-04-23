using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using ResuniqAI.Data;
using ResuniqAI.Models;
using ResuniqAI.ViewModels;
using System.Data;

namespace ResuniqAI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<ProfileController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await EnsureProfileStorageAsync();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var profile = await GetStoredProfileAsync(user) ?? BuildDefaultProfile(user);
            var resumes = await GetUserResumesSafeAsync(user.Id);
            var payments = await GetUserPaymentsSafeAsync(user.Email ?? string.Empty);

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

            UserProfile? profile;

            try
            {
                profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            }
            catch (Exception ex) when (IsSchemaIssue(ex))
            {
                _logger.LogWarning(ex, "Profile storage query failed for user {UserId}. Falling back to a new profile record.", user.Id);
                profile = null;
            }

            if (profile == null)
            {
                profile = BuildDefaultProfile(user);
                _context.UserProfiles.Add(profile);
            }

            var postedProfile = model.Profile ?? new UserProfile();

            profile.UserId = user.Id;
            profile.FullName = Normalize(postedProfile.FullName);
            profile.Headline = Normalize(postedProfile.Headline);
            profile.Location = Normalize(postedProfile.Location);
            profile.Phone = Normalize(postedProfile.Phone);
            profile.LinkedIn = Normalize(postedProfile.LinkedIn);
            profile.Github = Normalize(postedProfile.Github);
            profile.Portfolio = Normalize(postedProfile.Portfolio);
            profile.Bio = Normalize(postedProfile.Bio);
            profile.UpdatedAt = DateTime.Now;

            var normalizedEmail = (model.Email ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(normalizedEmail) && !string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = normalizedEmail;
                user.UserName = normalizedEmail;
            }

            if (!string.Equals(user.PhoneNumber, profile.Phone, StringComparison.Ordinal))
                user.PhoneNumber = profile.Phone;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(x => x.Description));
                ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errors) ? "Unable to update your account right now." : errors);

                model.Profile = profile;
                model.Email = user.Email ?? normalizedEmail;
                model.IsPro = await _userManager.IsInRoleAsync(user, "Pro");
                var resumes = await GetUserResumesSafeAsync(user.Id);
                var payments = await GetUserPaymentsSafeAsync(user.Email ?? string.Empty);

                model.ResumeCount = resumes.Count;
                model.PremiumResumeCount = resumes.Count(x => x.TemplateKey != null && !x.TemplateKey.StartsWith("ats-", StringComparison.OrdinalIgnoreCase));
                model.PaymentCount = payments.Count;
                model.ApprovedPaymentCount = payments.Count(x => x.IsApproved);
                model.Resumes = resumes.Take(8).ToList();
                model.IdentityUser = user;

                return View(model);
            }

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

        private static string Normalize(string? value) => value?.Trim() ?? string.Empty;

        private async Task<UserProfile?> GetStoredProfileAsync(IdentityUser user)
        {
            try
            {
                return await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            }
            catch (Exception ex) when (IsSchemaIssue(ex))
            {
                _logger.LogWarning(ex, "Profile query failed for user {UserId}. Returning default profile.", user.Id);
                return null;
            }
        }

        private async Task<List<Resume>> GetUserResumesSafeAsync(string userId)
        {
            try
            {
                return await _context.Resumes
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex) when (IsSchemaIssue(ex))
            {
                _logger.LogWarning(ex, "Resume query failed for profile dashboard user {UserId}. Returning no resumes.", userId);
                return new List<Resume>();
            }
        }

        private async Task<List<Payment>> GetUserPaymentsSafeAsync(string userEmail)
        {
            try
            {
                return await _context.Payments
                    .Where(x => x.UserEmail == userEmail)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex) when (IsSchemaIssue(ex))
            {
                _logger.LogWarning(ex, "Payment query failed for profile dashboard email {Email}. Returning no payments.", userEmail);
                return new List<Payment>();
            }
        }

        private static bool IsSchemaIssue(Exception ex)
        {
            if (ex is SqliteException)
                return true;

            return ex.InnerException is SqliteException;
        }

        private async Task EnsureProfileStorageAsync()
        {
            var connection = _context.Database.GetDbConnection();
            var shouldCloseConnection = connection.State != ConnectionState.Open;

            if (shouldCloseConnection)
                await connection.OpenAsync();

            try
            {
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
            finally
            {
                if (shouldCloseConnection)
                    await connection.CloseAsync();
            }
        }
    }
}
