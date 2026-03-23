using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using webappi.Data;
using webappi.Services.Interfaces;
using webappi.Services.Core;

var builder = WebApplication.CreateBuilder(args);

// Services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<AppUser, AppRole>(options => {
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3; // Minimal length for testing
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddDataProtection();
builder.Services.AddSingleton<IUrlEncryptionService, UrlEncryptionService>();

builder.Services.AddControllersWithViews();


var app = builder.Build();

// Http Pipeline


app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<webappi.Middleware.UrlObfuscationMiddleware>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<webappi.Middleware.SubscriptionMiddleware>();
app.UseMiddleware<webappi.Middleware.DynamicAuthorizationMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// DB Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();


    // context.Database.EnsureCreated();


    context.Database.Migrate();

    try 
    {
        // Roles
        if (!await roleManager.RoleExistsAsync("SuperAdmin")) await roleManager.CreateAsync(new AppRole { Name = "SuperAdmin" });
        if (!await roleManager.RoleExistsAsync("Admin")) await roleManager.CreateAsync(new AppRole { Name = "Admin" });


        if (await userManager.FindByEmailAsync("superadmin@erp.com") == null)
        {
            var user = new AppUser 
            { 
                UserName = "superadmin@erp.com", 
                Email = "superadmin@erp.com",
                FullName = "Super Administrator",
                UserType = "SuperAdmin",
                EmailConfirmed = true 
            };
            var result = await userManager.CreateAsync(user, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }

        // Role check for admins
        var orphans = await userManager.Users.Where(u => u.UserType == "Admin").ToListAsync();
        foreach (var orphan in orphans)
        {
            if (!await userManager.IsInRoleAsync(orphan, "Admin"))
            {
                await userManager.AddToRoleAsync(orphan, "Admin");
            }
        }

        // ERP Modules
        if (!context.ErpModules.Any())
        {
            context.ErpModules.AddRange(
                new ErpModule { ModuleName = "Student Information System", ModuleCode = "SIS", Description = "Manage student records" },
                new ErpModule { ModuleName = "Academic & Learning", ModuleCode = "ALM", Description = "Curriculum and LMS" },
                new ErpModule { ModuleName = "Fee & Finance", ModuleCode = "FINANCE", Description = "Fee collection" },
                new ErpModule { ModuleName = "Examination & Scorecard", ModuleCode = "ESM", Description = "Exams and grades" },
                new ErpModule { ModuleName = "Staff Management", ModuleCode = "STAFF", Description = "HR and Payroll" },
                new ErpModule { ModuleName = "Transport & Hostel", ModuleCode = "THM", Description = "Bus and Hostel" },
                new ErpModule { ModuleName = "Communication", ModuleCode = "COMM", Description = "Chats and Notices" },
                new ErpModule { ModuleName = "Administration", ModuleCode = "ADMIN", Description = "System Settings" },
                new ErpModule { ModuleName = "Super Admin Tools", ModuleCode = "SUPERADMIN", Description = "Core System Management" }
            );
            context.SaveChanges();
        }
        else
        {

            if (!context.ErpModules.Any(m => m.ModuleCode == "SUPERADMIN"))
            {
                context.ErpModules.Add(new ErpModule { ModuleName = "Super Admin Tools", ModuleCode = "SUPERADMIN", Description = "Core System Management" });
                context.SaveChanges();
            }
        }

        // ERP Pages
        if (!context.ErpPages.Any())
        {

            int GetModId(string code) => context.ErpModules.FirstOrDefault(m => m.ModuleCode == code)?.Id ?? 0;

            var pages = new List<ErpPage>();


            int sisId = GetModId("SIS");
            if (sisId > 0)
            {
                pages.Add(new ErpPage { ModuleId = sisId, PageName = "Student Registration", PageUrl = "/SIS/Registration", IsActive = true });
                pages.Add(new ErpPage { ModuleId = sisId, PageName = "Student List", PageUrl = "/SIS/List", IsActive = true });
                pages.Add(new ErpPage { ModuleId = sisId, PageName = "Attendance", PageUrl = "/SIS/Attendance", IsActive = true });
            }

            // SuperAdmin Tools
            int saId = GetModId("SUPERADMIN");
            if (saId > 0)
            {
                pages.Add(new ErpPage { ModuleId = saId, PageName = "Dashboard", PageUrl = "/SuperAdmin", IsActive = true });
                pages.Add(new ErpPage { ModuleId = saId, PageName = "Clients", PageUrl = "/SuperAdmin/Clients", IsActive = true });
                pages.Add(new ErpPage { ModuleId = saId, PageName = "Modules", PageUrl = "/SuperAdmin/Modules", IsActive = true });
                pages.Add(new ErpPage { ModuleId = saId, PageName = "Pages", PageUrl = "/SuperAdmin/Pages", IsActive = true });

                pages.Add(new ErpPage { ModuleId = saId, PageName = "Session Logs", PageUrl = "/Session/Index", IsActive = true });
            }

            if (pages.Any())
            {
                context.ErpPages.AddRange(pages);
                context.SaveChanges();
            }
        }
        else 
        {
            // Verify SA pages exist
            var saMod = context.ErpModules.FirstOrDefault(m => m.ModuleCode == "SUPERADMIN");
            if (saMod != null)
            {
                var existingPages = context.ErpPages.Where(p => p.ModuleId == saMod.Id).Select(p => p.PageUrl).ToList();
                var requiredPages = new List<ErpPage>
                {
                    new ErpPage { ModuleId = saMod.Id, PageName = "Dashboard", PageUrl = "/SuperAdmin", IsActive = true },
                    new ErpPage { ModuleId = saMod.Id, PageName = "Clients", PageUrl = "/SuperAdmin/Clients", IsActive = true },
                    new ErpPage { ModuleId = saMod.Id, PageName = "Modules", PageUrl = "/SuperAdmin/Modules", IsActive = true },
                    new ErpPage { ModuleId = saMod.Id, PageName = "Pages", PageUrl = "/SuperAdmin/Pages", IsActive = true },
                    new ErpPage { ModuleId = saMod.Id, PageName = "Session Logs", PageUrl = "/Session/Index", IsActive = true }
                };

                foreach (var p in requiredPages)
                {
                    if (!existingPages.Contains(p.PageUrl))
                    {
                        context.ErpPages.Add(p);
                    }
                }
                context.SaveChanges();
            }
        }

        // SA Permissions

        var superAdminUser = await userManager.FindByEmailAsync("superadmin@erp.com");
        if (superAdminUser != null)
        {
            var saMod = context.ErpModules.FirstOrDefault(m => m.ModuleCode == "SUPERADMIN");
            if (saMod != null)
            {
                var saPages = context.ErpPages.Where(p => p.ModuleId == saMod.Id).ToList();
                foreach (var page in saPages)
                {

                    var right = await context.ErpUserPageRights
                        .FirstOrDefaultAsync(r => r.UserId == superAdminUser.Id && r.PageId == page.Id);
                    
                    if (right == null)
                    {
                        context.ErpUserPageRights.Add(new ErpUserPageRight 
                        { 
                            UserId = superAdminUser.Id, 
                            PageId = page.Id, 
                            IsActive = true 
                        });
                    }
                    else if (!right.IsActive)
                    {

                        right.IsActive = true;
                        context.Entry(right).State = EntityState.Modified;
                    }
                }
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding Error: {ex.Message}");
    }

    try
    {
        // Token Integrity
        var pages = context.ErpPages.ToList(); // Fetch all first to process in memory
        var duplicateGroups = pages.GroupBy(p => p.UrlToken).Where(g => g.Count() > 1);
        
        bool changed = false;
        foreach (var group in duplicateGroups)
        {
            foreach (var page in group)
            {

                page.UrlToken = Guid.NewGuid().ToString("N");
                changed = true;
            }
        }
        
        if (pages.Any(p => string.IsNullOrEmpty(p.UrlToken)))
        {
             foreach (var p in pages.Where(p => string.IsNullOrEmpty(p.UrlToken)))
             {
                 p.UrlToken = Guid.NewGuid().ToString("N");
                 changed = true;
             }
        }

        if (changed) context.SaveChanges();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Token Healing Error: {ex.Message}");
    }

    try
    {
        // Missing Rights Repair
        var subscriptions = await context.ErpSubscriptions.Where(s => s.IsActive).ToListAsync();
        bool rightsChanged = false;

        foreach (var sub in subscriptions)
        {
            var modulePages = await context.ErpPages.Where(p => p.ModuleId == sub.ModuleId && p.IsActive).ToListAsync();
            foreach (var page in modulePages)
            {
                var hasRight = await context.ErpUserPageRights
                    .AnyAsync(r => r.UserId == sub.UserId && r.PageId == page.Id);
                
                if (!hasRight)
                {
                    context.ErpUserPageRights.Add(new ErpUserPageRight
                    {
                        UserId = sub.UserId,
                        PageId = page.Id,
                        IsActive = true
                    });
                    rightsChanged = true;
                }
            }
        }
        if (rightsChanged) await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Rights Healing Error: {ex.Message}");
    }

    try
    {
        // Designations
        if (!context.ErpDesignations.Any())
        {
            context.ErpDesignations.AddRange(
                new ErpDesignation { Name = "Administrator", Description = "System Administrator" },
                new ErpDesignation { Name = "Principal", Description = "School Principal" },
                new ErpDesignation { Name = "Teacher", Description = "Academic Staff" },
                new ErpDesignation { Name = "Accountant", Description = "Finance Staff" },
                new ErpDesignation { Name = "Student", Description = "Student" },
                new ErpDesignation { Name = "Staff", Description = "General Staff" }
            );
            context.SaveChanges();
        }

        // Admin Hierarchy
        var superAdmin = await userManager.FindByEmailAsync("superadmin@erp.com");
        if (superAdmin != null)
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                if (admin.ParentUserId == null && admin.Id != superAdmin.Id)
                {
                    admin.ParentUserId = superAdmin.Id;

                    if (admin.DesignationId == null)
                    {
                         var adminDesig = await context.ErpDesignations.FirstOrDefaultAsync(d => d.Name == "Administrator");
                         if (adminDesig != null) admin.DesignationId = adminDesig.Id;
                    }
                    await userManager.UpdateAsync(admin);
                }
            }
        }
    }
    catch (Exception ex)
    {
         Console.WriteLine($"Hierarchy Seeding Error: {ex.Message}");
    }
}

app.Run();