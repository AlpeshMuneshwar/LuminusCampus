using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace webappi.Controllers.SIS
{
    // Ensure route is SIS/[action] regardless of folder structure
    [Route("SIS/[action]")]
    public class SISController : Controller
    {
        [HttpGet]
        [Route("~/SIS")] // Handle /SIS root (Index) explicitly if needed, or rely on [action]
        [Route("Index")] // Explicitly map Index
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registration() => View();
        public IActionResult List() => View();
        public IActionResult Attendance() => View();
        public IActionResult Enrolling() => View();
    }
}
