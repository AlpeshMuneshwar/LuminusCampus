using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webappi.Data;
using webappi.Models; 
using System.Security.Claims;

namespace webappi.Controllers.ADMIN
{
    [Authorize(Roles = "Admin")] // Only Admins can create Staff
    [Route("ADMIN/Users/[action]")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> CreateEmployee()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // 1. Get Admin's Subscriptions
            var adminSubs = await _context.ErpSubscriptions
                .Include(s => s.Module)
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            // 2. Get Admin's Page Rights (if any)
            var adminPageRights = await _context.ErpUserPageRights
                .Where(r => r.UserId == userId && r.IsActive)
                .Select(r => r.PageId)
                .ToListAsync();

            var viewModel = new CreateUserViewModel
            {
                AvailableModules = new List<ModuleViewModel>(),
                CreatedUsers = await _userManager.Users.Where(u => u.CreatedBy == User.Identity.Name).ToListAsync()
            };

            foreach (var sub in adminSubs)
            {
                if (sub.Module == null) continue;

                var moduleVm = new ModuleViewModel
                {
                    ModuleId = sub.ModuleId,
                    ModuleName = sub.Module.ModuleName,
                    AvailablePages = new List<PageViewModel>()
                };

                // Fetch pages for this module
                var pages = await _context.ErpPages
                    .Where(p => p.ModuleId == sub.ModuleId && p.IsActive)
                    .ToListAsync();

                // 3. Filter Pages: If Admin has rights, show only those. If Admin has 0 rights (legacy), show all.
                if (adminPageRights.Any())
                {
                    pages = pages.Where(p => adminPageRights.Contains(p.Id)).ToList();
                }

                moduleVm.AvailablePages = pages.Select(p => new PageViewModel
                {
                    PageId = p.Id,
                    PageName = p.PageName
                }).ToList();

                viewModel.AvailableModules.Add(moduleVm);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Create User
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                UserType = "Staff", // Distinct type for granular access
                EmailConfirmed = true,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Assign Role
                if (!await _roleManager.RoleExistsAsync("Staff"))
                {
                    await _roleManager.CreateAsync(new AppRole { Name = "Staff" });
                }
                await _userManager.AddToRoleAsync(user, "Staff");

                // Assign Modules & Pages
                foreach (var mod in model.AvailableModules.Where(m => m.IsSelected))
                {
                    // 1. Create Subscription
                    _context.ErpSubscriptions.Add(new ErpSubscription
                    {
                        UserId = user.Id,
                        ModuleId = mod.ModuleId,
                        StartDate = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddYears(1),
                        IsActive = true
                    });

                    // 2. Create Page Rights
                    foreach (var page in mod.AvailablePages.Where(p => p.IsSelected))
                    {
                        _context.ErpUserPageRights.Add(new ErpUserPageRight
                        {
                            UserId = user.Id,
                            PageId = page.PageId,
                            IsActive = true,
                            AssignedBy = User.Identity.Name
                        });
                    }
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home"); // Or user list if exists
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userToEdit = await _userManager.FindByIdAsync(id.ToString());

            if (userToEdit == null || userToEdit.CreatedBy != User.Identity.Name)
            {
                return NotFound();
            }

            // 1. Get Admin's Subscriptions (What they can assign)
            var adminSubs = await _context.ErpSubscriptions
                .Include(s => s.Module)
                .Where(s => s.UserId == currentUser.Id && s.IsActive)
                .ToListAsync();

            // 2. Get Admin's Page Rights
            var adminPageRights = await _context.ErpUserPageRights
                .Where(r => r.UserId == currentUser.Id && r.IsActive)
                .Select(r => r.PageId)
                .ToListAsync();

            // 3. Get User's Existing Subscriptions & Rights
            var userSubs = await _context.ErpSubscriptions
                .Where(s => s.UserId == id && s.IsActive)
                .Select(s => s.ModuleId)
                .ToListAsync();

            var userPageRights = await _context.ErpUserPageRights
                .Where(r => r.UserId == id && r.IsActive)
                .Select(r => r.PageId)
                .ToListAsync();

            var viewModel = new CreateUserViewModel
            {
                UserId = userToEdit.Id,
                FullName = userToEdit.FullName,
                Email = userToEdit.Email,
                AvailableModules = new List<ModuleViewModel>()
            };

            foreach (var sub in adminSubs)
            {
                if (sub.Module == null) continue;

                var moduleVm = new ModuleViewModel
                {
                    ModuleId = sub.ModuleId,
                    ModuleName = sub.Module.ModuleName,
                    IsSelected = userSubs.Contains(sub.ModuleId),
                    AvailablePages = new List<PageViewModel>()
                };

                var pages = await _context.ErpPages
                    .Where(p => p.ModuleId == sub.ModuleId && p.IsActive)
                    .ToListAsync();

                if (adminPageRights.Any())
                {
                    pages = pages.Where(p => adminPageRights.Contains(p.Id)).ToList();
                }

                moduleVm.AvailablePages = pages.Select(p => new PageViewModel
                {
                    PageId = p.Id,
                    PageName = p.PageName,
                    IsSelected = userPageRights.Contains(p.Id)
                }).ToList();

                viewModel.AvailableModules.Add(moduleVm);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateUserViewModel model)
        {
            // Note: Password validation skipped for Edit
            // ModelState.Remove("Password"); 

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null || user.CreatedBy != User.Identity.Name)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            // user.Email = model.Email; // Typically email isn't changed so easily, but if needed:
            // await _userManager.SetEmailAsync(user, model.Email);

            await _userManager.UpdateAsync(user);

            // Update Rights
            foreach (var mod in model.AvailableModules)
            {
                // 1. Handle Module Subscription
                var existingSub = await _context.ErpSubscriptions
                    .FirstOrDefaultAsync(s => s.UserId == user.Id && s.ModuleId == mod.ModuleId);

                if (mod.IsSelected)
                {
                    if (existingSub == null)
                    {
                        _context.ErpSubscriptions.Add(new ErpSubscription
                        {
                            UserId = user.Id,
                            ModuleId = mod.ModuleId,
                            StartDate = DateTime.UtcNow,
                            ExpiryDate = DateTime.UtcNow.AddYears(1),
                            IsActive = true
                        });
                    }
                    else
                    {
                        existingSub.IsActive = true;
                    }

                    // 2. Handle Pages
                    foreach (var page in mod.AvailablePages)
                    {
                        var existingRight = await _context.ErpUserPageRights
                            .FirstOrDefaultAsync(r => r.UserId == user.Id && r.PageId == page.PageId);

                        if (page.IsSelected)
                        {
                            if (existingRight == null)
                            {
                                _context.ErpUserPageRights.Add(new ErpUserPageRight
                                {
                                    UserId = user.Id,
                                    PageId = page.PageId,
                                    IsActive = true,
                                    AssignedBy = User.Identity.Name
                                });
                            }
                            else
                            {
                                existingRight.IsActive = true;
                            }
                        }
                        else
                        {
                            if (existingRight != null) existingRight.IsActive = false;
                        }
                    }
                }
                else
                {
                    // If module unselected, disable subscription and all page rights
                    if (existingSub != null) existingSub.IsActive = false;
                    
                    var modulePageIds = mod.AvailablePages.Select(p => p.PageId).ToList();
                    var rightsToRemove = await _context.ErpUserPageRights
                        .Where(r => r.UserId == user.Id && modulePageIds.Contains(r.PageId))
                        .ToListAsync();
                    
                    foreach(var r in rightsToRemove) r.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateEmployee"); // Return to list/create page
        }
    }

    public class CreateUserViewModel
    {
        public int UserId { get; set; } // For Edit
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; } // Nullable for Edit
        public List<ModuleViewModel> AvailableModules { get; set; } = new();
        public List<AppUser> CreatedUsers { get; set; } = new(); // For List
    }

    public class ModuleViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool IsSelected { get; set; }
        public List<PageViewModel> AvailablePages { get; set; } = new();
    }

    public class PageViewModel
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public bool IsSelected { get; set; }
    }
}
