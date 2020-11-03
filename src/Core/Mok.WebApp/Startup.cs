using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mok.Blog.Services.Interfaces;
using Mok.Data;
using Scrutor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mok.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // DbCtx
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Caching
            services.AddDistributedMemoryCache();

            // Scrutor 
            services.Scan(scan => scan
              .FromAssembliesOf(typeof(ICategoryService))
              .AddClasses()
              .UsingRegistrationStrategy(RegistrationStrategy.Skip) // prevent added to add again
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.RootDirectory = "/Manage";
                });

            // JsonConvert
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // To make ajax work with razor pages
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (!db.Database.ProviderName.Equals("Microsoft.EntityFrameworkCore.InMemory"))
                db.Database.Migrate();
        }
    }
}
