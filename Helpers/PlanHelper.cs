using Microsoft.EntityFrameworkCore;
using ResuniqAI.Data;

namespace ResuniqAI.Helpers
{
    public class PlanHelper
    {
        private readonly ApplicationDbContext _context;

        public PlanHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsPro(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

var sub = await _context.Subscriptions
    .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == true);

            if (sub == null)
                return false;

            return sub.ExpireDate > DateTime.Now;
        }
    }
}