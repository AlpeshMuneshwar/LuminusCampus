using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webappi.Data;

namespace webappi.Middleware
{
    public class DynamicAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DynamicAuthorizationMiddleware> _logger;

        public DynamicAuthorizationMiddleware(RequestDelegate next, ILogger<DynamicAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var path = context.Request.Path.Value?.ToLower().TrimEnd('/');

            // Exclusions
            if (path == "/" || 
                path.StartsWith("/css") || 
                path.StartsWith("/js") || 
                path.StartsWith("/lib") || 
                path.StartsWith("/account") || // Login/Logout
                path.StartsWith("/api/auth") ||
                path == "/home/accessdenied" || 
                path == "/home/error")
            {
                await _next(context);
                return;
            }

            // Auth Check
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }



            // Page Lookup
            

            
            var page = await dbContext.ErpPages
                .FirstOrDefaultAsync(p => p.PageUrl.ToLower() == path && p.IsActive);

            if (page != null)
            {
                // Check Rights
                var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                var hasRight = await dbContext.ErpUserPageRights
                    .AnyAsync(r => r.UserId == userId && r.PageId == page.Id && r.IsActive);

                if (!hasRight)
                {
                    _logger.LogWarning($"Access Denied for User {userId} to {path}");
                    
                    if (IsAjaxRequest(context.Request))
                    {
                        context.Response.StatusCode = 403;
                        return;
                    }
                        context.Response.Redirect("/Account/AccessDenied");
                        return;
                }
            }



            await _next(context);
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
