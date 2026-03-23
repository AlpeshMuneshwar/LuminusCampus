using Microsoft.EntityFrameworkCore;
using webappi.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using webappi.Services.Core;

namespace webappi.Middleware
{
    public class UrlObfuscationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UrlObfuscationMiddleware> _logger;
        private readonly IUrlEncryptionService _encryptionService;

        public UrlObfuscationMiddleware(RequestDelegate next, ILogger<UrlObfuscationMiddleware> logger, IUrlEncryptionService encryptionService)
        {
            _next = next;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var path = context.Request.Path.Value?.TrimEnd('/');
            

            if (path != null && path.StartsWith("/q/", StringComparison.OrdinalIgnoreCase))
            {
                var token = path.Substring(3);
                if (!string.IsNullOrEmpty(token))
                {
                    // Dynamic Decryption
                    var decryptedPath = _encryptionService.Decrypt(token);
                    
                    if (!string.IsNullOrEmpty(decryptedPath))
                    {
                        context.Request.Path = decryptedPath;
                        _logger.LogInformation($"Obfuscation (Dynamic): Rewrote /q/... -> {decryptedPath}");
                    }
                    else
                    {
                        // DB Fallback
                        var page = await dbContext.ErpPages
                            .FirstOrDefaultAsync(p => p.UrlToken == token);

                        if (page != null)
                        {
                            context.Request.Path = page.PageUrl;
                            _logger.LogInformation($"Obfuscation (Legacy DB): Rewrote {path} -> {page.PageUrl}");
                        }
                        else
                        {
                            _logger.LogWarning($"Obfuscation: Invalid token {token}");

                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
