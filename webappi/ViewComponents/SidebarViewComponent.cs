using Microsoft.AspNetCore.Mvc;
using webappi.Services.Interfaces;
using System.Security.Claims;
// using webappi.Models;

namespace webappi.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public SidebarViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userIdStr = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        int.TryParse(userIdStr, out int userId);
        
        // Find role (assuming single role for now or primary)
        var role = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? "Student"; 

        var sections = await _menuService.GetMenuForUserAsync(userId, role);
        return View(sections);
    }
}
