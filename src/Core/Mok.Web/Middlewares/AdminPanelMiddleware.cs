using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mok.Web.Middlewares
{
    public class AdminPanelMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminPanelMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var setupUrl = $"{context.Request.Scheme}://{context.Request.Host}/admin/categories";
            var currentUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";

            // don't redirect to setup if url is setup itself or certain types of files
            string[] exts = { ".ico", ".js", ".css", ".map" };
            if (!currentUrl.Equals(setupUrl, StringComparison.OrdinalIgnoreCase) &&
                !exts.Any(ext => currentUrl.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                context.Response.Redirect(setupUrl);
                return;
            }
        }
    }
}
