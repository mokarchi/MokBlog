using Mok.Web.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="SetupMiddleware"/> to check if site needs to be setup.
        /// </summary>
        /// <param name="app">Builder for configuring an application's request pipeline</param>
        public static IApplicationBuilder UseSetup(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SetupMiddleware>();
        }
    }
}
