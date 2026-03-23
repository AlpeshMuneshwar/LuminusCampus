using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace webappi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Index", "SuperAdmin");
                }
                // Regular Admin Dashboard
                return View();
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
