using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webappi.Data;
using webappi.Models;

namespace webappi.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Session Management
                        var activeSessions = _context.UserSessions
                            .Where(s => s.UserId == user.Id && s.IsActive)
                            .OrderBy(s => s.LoginTime)
                            .ToList();

                        if (activeSessions.Count >= 2)
                        {
                            var oldest = activeSessions.First();
                            oldest.IsActive = false;
                            oldest.LogoutTime = DateTime.UtcNow;
                        }

                        var newSession = new webappi.Data.UserSession
                        {
                            UserId = user.Id,
                            Token = Guid.NewGuid().ToString(),
                            LoginTime = DateTime.UtcNow,
                            LastSeen = DateTime.UtcNow,
                            IsActive = true,
                            IPAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Browser = Request.Headers["User-Agent"].ToString(),
                            Device = "Unknown"
                        };

                        _context.UserSessions.Add(newSession);
                        await _context.SaveChangesAsync();


                        if (await _signInManager.UserManager.IsInRoleAsync(user, "SuperAdmin"))
                        {
                            return RedirectToAction("Index", "SuperAdmin");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
