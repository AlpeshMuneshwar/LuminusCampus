using webappi.Data;

namespace webappi.Middleware
{
    public class SessionTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // We assume auth cookie/token has some identifier, or we track by User Name + IP + UserAgent approximation
                    // For better precision, we'd store a SessionId claim on login.
                    // For now, let's update all active sessions for this user that match the IP/UserAgent
                    // OR simpler: just update "LastSeen" for the user's latest active session?
                    // NOTE: Without a SessionId claim, distinguishing strict sessions is hard. 
                    // Let's assume we match by UserName and IP/UserAgent for now.
                    
                    var username = context.User.Identity.Name;
                    var ip = context.Connection.RemoteIpAddress?.ToString();
                    var ua = context.Request.Headers["User-Agent"].ToString();

                    var session = db.UserSessions
                        .Where(s => s.User.UserName == username && s.IsActive && s.IPAddress == ip && s.Browser == ua)
                        .OrderByDescending(s => s.LoginTime)
                        .FirstOrDefault();

                    if (session != null)
                    {
                        session.LastSeen = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                    }
                }
            }

            await _next(context);
        }
    }
}
