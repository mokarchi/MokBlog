using AutoMapper;
using Mok.Data;
using Mok.Settings;
using Mok.Blog.Models;
using Mok.Blog.Helpers;
using Mok.Membership;
using Mok.Navigation;
using Mok.Blog.Services;
using Mok.Web.Extensions;
using Mok.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scrutor;
using System.IO;
using System.Linq;

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

            // Identity
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Cookie https://bit.ly/2FNyPnr
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/denied";
            });

            // Caching
            services.AddDistributedMemoryCache();

            // AutoMapper
            services.AddAutoMapper(typeof(BlogPost));
            services.AddSingleton(BlogUtil.Mapper);

            // Mediatr
            services.AddMediatR(typeof(BlogPost));

            // Storage
            services.AddStorageProvider(Configuration);

            // Plugins
            //services.AddPlugins(Env);

            // Scrutor 
            services.Scan(scan => scan
              .FromAssembliesOf(typeof(ISettingService), typeof(BlogSettings))
              .AddClasses()
              .UsingRegistrationStrategy(RegistrationStrategy.Skip) // prevent added to add again
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            services.AddScoped<INavProvider, PageService>();
            services.AddScoped<INavProvider, CategoryService>();

            // Authorization
            // if you update the roles and find the app not working, try logout then login https://stackoverflow.com/a/48177723/32240
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRoles", policy => policy.RequireRole("Administrator", "Editor"));
            });

            // HttpContext
            services.AddHttpContextAccessor();

            // MVC, Razor Pages, TempData, Json.net
            var builder = services.AddMvc() // https://bit.ly/2XTLFZB
                .AddApplicationPart(typeof(HomeController).Assembly) // https://bit.ly/2Zbbe8I
                .AddSessionStateTempDataProvider()
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddRazorPagesOptions(options =>
                {
                    options.RootDirectory = "/Manage";
                    options.Conventions.AuthorizeFolder("/Admin", "AdminRoles");
                    options.Conventions.AuthorizeFolder("/Plugins", "AdminRoles");
                    options.Conventions.AuthorizeFolder("/Widgets", "AdminRoles");
                });

            services.AddSession(); // for TempData only

            // JsonConvert
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // To make ajax work with razor pages
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            // RCLs to monitor
            //if (Env.IsDevelopment())
            //{
            //    string[] extDirs = { "Plugins", "SysPlugins", "Themes", "Widgets" };
            //    string[] extPaths = { };

            //    foreach (var extDir in extDirs)
            //    {
            //        var dirPath = Directory.GetDirectories(Path.GetFullPath(Path.Combine(Env.ContentRootPath, "..", "..", extDir)));
            //        extPaths = extPaths.Concat(dirPath).ToArray();
            //    }

            //    builder.AddRazorRuntimeCompilation(options =>
            //    {
            //        foreach (var path in extPaths)
            //        {
            //            options.FileProviders.Add(new PhysicalFileProvider(path));
            //        }
            //    });
            //}
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                #if DEBUG
                app.UseBrowserLink();
                #endif
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSetup();
            app.MapWhen(context => context.Request.Path.ToString().Equals("/olw"), appBuilder => appBuilder.UseMetablog());
            app.UseStatusCodePagesWithReExecute("/Home/ErrorCode/{0}"); // needs to be after hsts and rewrite
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseSession(); // for TempData only

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Home", "", new { controller = "Home", action = "Index" });
                BlogRoutes.RegisterRoutes(endpoints);
                endpoints.MapControllerRoute(name: "Default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (!db.Database.ProviderName.Equals("Microsoft.EntityFrameworkCore.InMemory"))
                db.Database.Migrate();
        }
    }
}
