using Microsoft.AspNetCore.Mvc;

namespace ResuniqAI.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}