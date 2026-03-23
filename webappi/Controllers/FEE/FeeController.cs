using Microsoft.AspNetCore.Mvc;

namespace webappi.Controllers.FEE
{
    [Route("Fee/[action]")]
    public class FeeController : Controller
    {
        [HttpGet]
        [Route("~/Fee")] // Handle /SIS root (Index) explicitly if needed, or rely on [action]
        [Route("TempPage")] // Explicitly map Index
        public IActionResult TempPage()
        {
            return View();
        }
    }
}

