using Mok.Blog.MetaWeblog;
using Mok.Web.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="MetaWeblogMiddleware"/> to the application's request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMetablog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MetaWeblogMiddleware>();
        }

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
