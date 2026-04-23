using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResuniqAI.Data;
using ResuniqAI.Models;

namespace ResuniqAI.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Upgrade() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(string transactionId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            if (string.IsNullOrWhiteSpace(transactionId))
            {
                TempData["UpgradeError"] = "Please enter a valid transaction or reference number.";
                return RedirectToAction(nameof(Upgrade));
            }

            var payment = new Payment
            {
                UserEmail = user.Email ?? "",
                TransactionId = transactionId.Trim(),
                IsApproved = false,
                CreatedAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Pending");
        }

        public IActionResult Pending()
        {
            return View();
        }
    }
}
