using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webappi.Data;
using webappi.Models;
using System.Security.Claims;

namespace webappi.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    [webappi.Attributes.SuperAdminOnly]
    public class SuperAdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SuperAdminController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalClients = await _userManager.Users.CountAsync(u => u.UserType == "Admin" && u.Id != 1), // Strict Admin Count
                TotalModules = await _context.ErpModules.CountAsync(m => m.IsActive),
                TotalPages = await _context.ErpPages.CountAsync(p => p.IsActive)
            };

            var modules = await _context.ErpModules.Where(m => m.IsActive).Include(m => m.ErpPages).ToListAsync();
            foreach (var mod in modules)
            {
                model.PagesPerModule.Add(mod.ModuleName, await _context.ErpPages.CountAsync(p => p.ModuleId == mod.Id && p.IsActive));
            }

            return View(model); 
        }

        // Clients
        [HttpGet]
        public async Task<IActionResult> Clients()
        {
            var users = await _userManager.Users
                .Where(u => u.Id != 1 && u.UserType == "Admin")
                .Include(u => u.ParentUser)
                .Include(u => u.Designation)
                .Where(u => u.Id != 1)
                .Select(u => new UserViewModel { 
                    Id = u.Id, 
                    FullName = u.FullName ?? "N/A", 
                    Email = u.Email ?? "N/A", 
                    UserType = u.UserType ?? "Admin",
                    Designation = u.Designation != null ? u.Designation.Name : "N/A",
                    ParentUserId = u.ParentUserId,
                    ParentName = u.ParentUser != null ? (u.ParentUser.FullName ?? u.ParentUser.Email) : "Super Administrator"
                })
                .ToListAsync();
            
            ViewBag.Modules = await _context.ErpModules.Where(m => m.IsActive).ToListAsync();
            ViewBag.Designations = await _context.ErpDesignations.Where(d => d.IsActive).ToListAsync();
            return View(users);
        }

        // Modules
        [HttpGet]
        public async Task<IActionResult> Modules()
        {
            var modules = await _context.ErpModules.ToListAsync();
            return View(modules);
        }

        [HttpPost]
        public async Task<IActionResult> CreateModule([FromForm] ErpModule module)
        {
            if (ModelState.IsValid)
            {
                _context.ErpModules.Add(module);
                await _context.SaveChangesAsync();
                return RedirectToAction("Modules");
            }
            return RedirectToAction("Modules");
        }

        // Pages
        [HttpGet]
        public async Task<IActionResult> Pages()
        {
            var pages = await _context.ErpPages.Include(p => p.Module).ToListAsync();
            ViewBag.Modules = await _context.ErpModules.Where(m => m.IsActive).ToListAsync();
            return View(pages);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePage([FromForm] ErpPage page)
        {
            if (ModelState.IsValid)
            {
                if (!page.PageUrl.StartsWith("/")) page.PageUrl = "/" + page.PageUrl;
                _context.ErpPages.Add(page);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Page created successfully.";
                return RedirectToAction("Pages");
            }
            var errors = string.Join("; ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            TempData["Error"] = $"Failed to create page. Errors: {errors}";
            return RedirectToAction("Pages");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePage([FromForm] ErpPage page)
        {
            if (ModelState.IsValid)
            {
                var existingPage = await _context.ErpPages.FindAsync(page.Id);
                if (existingPage != null)
                {
                    existingPage.PageName = page.PageName;
                    existingPage.PageUrl = page.PageUrl;
                    existingPage.ModuleId = page.ModuleId;
                    existingPage.Description = page.Description;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Page updated successfully.";
                }
            }
            else 
            {
                 TempData["Error"] = "Failed to update page. Please check input.";
            }
            return RedirectToAction("Pages");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(int id)
        {
            var page = await _context.ErpPages.FindAsync(id);
            if (page != null)
            {

                var rights = await _context.ErpUserPageRights.Where(r => r.PageId == id).ToListAsync();
                _context.ErpUserPageRights.RemoveRange(rights);
                
                _context.ErpPages.Remove(page);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Pages");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClient([FromForm] CreateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "1");

                var user = new AppUser 
                { 
                    UserName = request.Email, 
                    Email = request.Email, 
                    FullName = request.FullName,
                    UserType = "Admin",
                    EmailConfirmed = true,
                    ParentUserId = currentUserId,
                    DesignationId = request.DesignationId
                };
                
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    TempData["Success"] = "Client created successfully.";
                    return RedirectToAction("Clients");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            TempData["Error"] = "Error creating client. verify password complexity.";
            return RedirectToAction("Clients");
        }

        [HttpGet]
        public async Task<IActionResult> ManageRights(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var allModules = await _context.ErpModules.Where(m => m.IsActive).ToListAsync();
            var userSubs = await _context.ErpSubscriptions.Where(s => s.UserId == id).ToListAsync();

            var model = new ManageRightsViewModel
            {
                UserId = user.Id,
                UserName = user.FullName ?? user.Email,
                Modules = allModules.Select(m => new ModuleRightViewModel
                {
                    ModuleId = m.Id,
                    ModuleName = m.ModuleName,
                    IsSelected = userSubs.Any(s => s.ModuleId == m.Id),
                    StartDate = userSubs.FirstOrDefault(s => s.ModuleId == m.Id)?.StartDate,
                    ExpiryDate = userSubs.FirstOrDefault(s => s.ModuleId == m.Id)?.ExpiryDate
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRights(ManageRightsViewModel model)
        {
            // Remove existing
            var existing = await _context.ErpSubscriptions.Where(s => s.UserId == model.UserId).ToListAsync();
            _context.ErpSubscriptions.RemoveRange(existing);

            // Add new
            var newSubs = new List<ErpSubscription>();
            foreach (var m in model.Modules.Where(m => m.IsSelected))
            {
                newSubs.Add(new ErpSubscription
                {
                    UserId = model.UserId,
                    ModuleId = m.ModuleId,
                    StartDate = m.StartDate ?? DateTime.UtcNow,
                    ExpiryDate = m.ExpiryDate ?? DateTime.UtcNow.AddYears(1),
                    IsActive = true
                });
            }

            if (newSubs.Any())
            {
                await _context.ErpSubscriptions.AddRangeAsync(newSubs);
            }

            // Sync Page Rights
            var selectedModuleIds = model.Modules.Where(m => m.IsSelected).Select(m => m.ModuleId).ToList();
            var allModuleIds = model.Modules.Select(m => m.ModuleId).ToList();
            
            // Fetch all pages involved in this update (pages belonging to any module in the list)
            var allPagesInvolved = await _context.ErpPages
                .Where(p => allModuleIds.Contains(p.ModuleId))
                .ToListAsync();

            // Fetch existing rights for this user
            var existingRights = await _context.ErpUserPageRights
                .Where(r => r.UserId == model.UserId)
                .ToListAsync();

            foreach (var page in allPagesInvolved)
            {
                bool shouldHaveRight = selectedModuleIds.Contains(page.ModuleId);
                var right = existingRights.FirstOrDefault(r => r.PageId == page.Id);

                if (shouldHaveRight)
                {

                    if (right == null)
                    {
                        _context.ErpUserPageRights.Add(new ErpUserPageRight 
                        { 
                            UserId = model.UserId, 
                            PageId = page.Id, 
                            IsActive = true 
                        });
                    }
                    else if (!right.IsActive)
                    {
                        right.IsActive = true;
                        _context.Entry(right).State = EntityState.Modified;
                    }
                }
                else
                {

                    if (right != null)
                    {
                        _context.ErpUserPageRights.Remove(right);
                    }
                }
            }

            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Rights updated successfully.";

            return RedirectToAction("Clients");
        }
    }

}
