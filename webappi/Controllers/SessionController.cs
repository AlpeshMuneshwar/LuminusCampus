using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webappi.Data;

namespace webappi.Controllers
{
    [Authorize]
    [webappi.Attributes.SuperAdminOnly]
    public class SessionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SessionController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isSuperAdmin = await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

            List<UserSession> sessions;

            if (isSuperAdmin)
            {

                sessions = await _context.UserSessions
                    .Include(s => s.User)
                    .OrderByDescending(s => s.LastSeen ?? s.LoginTime)
                    .Take(100)
                    .ToListAsync();
            }
            else if (isAdmin)
            {

                
                sessions = await _context.UserSessions
                    .Include(s => s.User)
                    .Where(s => s.UserId == currentUser.Id || s.User.CreatedBy == currentUser.UserName)
                    .OrderByDescending(s => s.LastSeen ?? s.LoginTime)
                    .ToListAsync();
            }
            else
            {

                sessions = await _context.UserSessions
                    .Include(s => s.User)
                    .Where(s => s.UserId == currentUser.Id)
                    .OrderByDescending(s => s.LastSeen ?? s.LoginTime)
                    .ToListAsync();
            }

            return View(sessions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kill(int id)
        {
            var session = await _context.UserSessions.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (session == null) return NotFound();


            var currentUser = await _userManager.GetUserAsync(User);
            var isSuperAdmin = await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            bool canKill = false;
            if (isSuperAdmin) canKill = true;
            else if (isAdmin)
            {

                if (session.UserId == currentUser.Id || session.User.CreatedBy == currentUser.UserName)
                {
                    canKill = true;
                }
            }
            else
            {

                if (session.UserId == currentUser.Id) canKill = true;
            }

            if (canKill)
            {
                session.IsActive = false;
                session.LogoutTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Session terminated successfully.";
            }
            else
            {
                TempData["Error"] = "You do not have permission to terminate this session.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
