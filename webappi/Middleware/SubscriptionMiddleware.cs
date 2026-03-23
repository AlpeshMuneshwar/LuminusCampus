using System.Security.Claims;
using webappi.Data; // Valid based on file list
using Microsoft.EntityFrameworkCore;


namespace webappi.Middleware;

public class SubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {

        if (context.User.Identity?.IsAuthenticated != true || 
            context.Request.Path.StartsWithSegments("/Auth") ||
            context.Request.Path.StartsWithSegments("/Home/Error") ||
            context.Request.Path.StartsWithSegments("/Account") ||
            context.User.IsInRole("SuperAdmin"))
        {
            await _next(context);
            return;
        }

        var routeData = context.GetRouteData();
        var areaName = routeData?.Values["area"]?.ToString();
        
        if (string.IsNullOrEmpty(areaName))
        {
            var controllerName = routeData?.Values["controller"]?.ToString();
            if (!string.IsNullOrEmpty(controllerName))
            {
                areaName = controllerName.ToUpper();
            }
        }

        if (!string.IsNullOrEmpty(areaName))
        {
            var module = await dbContext.ErpModules.FirstOrDefaultAsync(m => m.ModuleCode == areaName);

            if (module != null)
            {
                var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var hasSubscription = await dbContext.ErpSubscriptions
                        .AnyAsync(s => s.UserId == userId && 
                                       s.ModuleId == module.Id && 
                                       s.IsActive && 
                                       s.ExpiryDate > DateTime.Now);

                    if (!hasSubscription)
                    {
                        context.Response.Redirect("/Account/AccessDenied"); 
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
