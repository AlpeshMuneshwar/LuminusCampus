using Microsoft.EntityFrameworkCore;
using webappi.Data;
using webappi.Services.Interfaces;

namespace webappi.Services.Core
{
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUrlEncryptionService _encryptionService;

        public MenuService(ApplicationDbContext context, IUrlEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<List<MenuSection>> GetMenuForUserAsync(int userId, string userRole)
        {
            var menu = new List<MenuSection>();

            
            // Role-based Menu Logic
            if (userRole == "Admin" || userRole == "Staff" || userRole == "SuperAdmin")
            {
                // Always show Dashboard
                menu.Add(new MenuSection
                {
                    Title = "", 
                    Items = new List<MenuItem>
                    {
                        new MenuItem { Title = "Dashboard", Url = userRole == "SuperAdmin" ? "/SuperAdmin" : "/Home", Icon = "lni lni-dashboard" }
                    }
                });

                if (userRole == "Admin")
                {
                    menu.Last().Items.Add(new MenuItem { Title = "Create Employee", Url = "/ADMIN/Users/CreateEmployee", Icon = "lni lni-users" });
                }

                // Fetch Access Rights
                
                List<ErpModule> visibleModules = new List<ErpModule>();

                if (userRole == "SuperAdmin")
                {
                    // SuperAdmin Page Rights
                    visibleModules = await _context.ErpUserPageRights
                        .Where(r => r.UserId == userId && r.IsActive)
                        .Include(r => r.Page)
                        .ThenInclude(p => p.Module)
                        .Select(r => r.Page.Module)
                        .Distinct()
                        .ToListAsync();
                }
                else
                {
                    // Subscriptions (for Admin/Staff)
                    visibleModules = await _context.ErpSubscriptions
                        .Include(s => s.Module)
                        .Where(s => s.UserId == userId && s.IsActive)
                        .Select(s => s.Module)
                        .ToListAsync();
                }


                List<int> authorizedPageIds = null;
                if (userRole == "Staff" || userRole == "SuperAdmin")
                {
                    authorizedPageIds = await _context.ErpUserPageRights
                        .Where(r => r.UserId == userId && r.IsActive)
                        .Select(r => r.PageId)
                        .ToListAsync();
                }

                // Group by Module
                var moduleSection = new MenuSection { Title = "", Items = new List<MenuItem>() };

                foreach (var module in visibleModules)
                {
                    if (module != null)
                    {

                        
                        var pages = await _context.ErpPages
                                .Where(p => p.ModuleId == module.Id && p.IsActive)
                                .ToListAsync();


                        if ((userRole == "Staff" || userRole == "SuperAdmin") && authorizedPageIds != null)
                        {
                            pages = pages.Where(p => authorizedPageIds.Contains(p.Id)).ToList();
                        }

                        var menuItem = new MenuItem 
                        { 
                            Title = module.ModuleName, 
                            Url = "#", 
                            Icon = "lni lni-grid-alt" 
                        };

                        if (pages.Any())
                        {
                             menuItem.SubItems = pages.Select(p => {
                                 // URL Obfuscation Token Generation
                                 var token = _encryptionService.Encrypt(p.PageUrl);
                                 return new MenuItem 
                                 {
                                     Title = p.PageName,
                                     Url = $"/q/{token}",
                                     Icon = "lni lni-empty-file"
                                 };
                             }).ToList();
                        }
                        
                        if (pages.Any() || userRole == "Admin") // Admins see module even if empty? maybe
                        {
                             moduleSection.Items.Add(menuItem);
                        }
                    }
                }

                if (moduleSection.Items.Any())
                {
                    menu.Add(moduleSection);
                }
            }

            return menu;
        }

        private string GetModuleUrl(string moduleCode)
        {
            // Simple mapping for now
            return moduleCode switch
            {
                "SIS" => "/SIS",
                "ALM" => "/ALM",
                "FINANCE" => "/Finance",
                _ => "/Home"
            };
        }
    }
}
