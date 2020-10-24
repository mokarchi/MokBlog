using Mok.Web.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAdminPanel(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AdminPanelMiddleware>();
        }
    }
}
