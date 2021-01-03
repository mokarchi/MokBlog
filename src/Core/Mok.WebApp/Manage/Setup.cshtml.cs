using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Mok.Blog.Enums;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Mok.Membership;
using Mok.Navigation;
using Mok.Settings;
using Mok.Themes;
using Newtonsoft.Json;

namespace Mok.WebApp.Manage
{
    public class SetupModel : PageModel
    {
        private readonly ISettingService settingService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ICategoryService _catSvc;
        private readonly IPageService pageService;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly INavigationService navigationService;
        private readonly ILogger<SetupModel> _logger;

        public const string SETUP_DATA_DIR = "Setup";

        public SetupModel(ISettingService settingService,
                          UserManager<User> userManager,
                          SignInManager<User> signInManager,
                          RoleManager<Role> roleManager,
                          ICategoryService catService,
                          IPageService pageService,
                          IWebHostEnvironment hostingEnvironment,
                          INavigationService navigationService,
                          ILogger<SetupModel> logger)
        {
            this.settingService = settingService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _catSvc = catService;
            this.pageService = pageService;
            this.hostingEnvironment = hostingEnvironment;
            this.navigationService = navigationService;
            _logger = logger;
        }

        public string Title { get; set; }
        public string TimeZoneId { get; set; }
        public List<SelectListItem> TimeZones { get; set; }
        public string TimeZonesJson { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Initializes setup and if setup has been done redirect to blog index page.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            var coreSettings = await settingService.GetSettingsAsync<CoreSettings>();
            if (coreSettings.SetupDone)
            {
                return RedirectToAction("Index", "Blog");
            }

            TimeZones = new List<SelectListItem>();
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                TimeZones.Add(new SelectListItem() { Value = tz.Id, Text = tz.DisplayName });
            }
            TimeZoneId = "UTC";
            TimeZonesJson = JsonConvert.SerializeObject(TimeZones);

            return Page();
        }

        /// <summary>
        /// Setting up site.
        /// </summary>
        /// <remarks>
        /// It creates the first user, system roles, assign Administrator role to the user, 
        /// core settings, first blog post and blog settings.
        /// </remarks>
        public async Task<IActionResult> OnPostAsync([FromBody] SetupModel model)
        {
            try
            {
                _logger.LogInformation("Fanray setup begins");

                var validator = new SetupValidator();
                var valResult = await validator.ValidateAsync(model);
                if (!valResult.IsValid)
                {
                    throw new MokException($"Failed to create blog.", valResult.Errors);
                }

                // first user
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    DisplayName = model.DisplayName
                };

                IdentityResult result = IdentityResult.Success;

                // create user if not found
                var foundUser = await _userManager.FindByEmailAsync(model.Email);
                if (foundUser == null)
                {
                    result = await _userManager.CreateAsync(user, model.Password);
                }
                else // update username
                {
                    foundUser.UserName = model.UserName;
                    await _userManager.UpdateNormalizedUserNameAsync(foundUser);
                }

                // create system roles
                if (result.Succeeded)
                {
                    _logger.LogInformation("{@User} account created.", user);

                    result = await CreateSystemRolesAsync();
                }

                // assign Administrator role to the user
                if (result.Succeeded)
                {
                    // get the actual user object before look up IsInRole
                    user = await _userManager.FindByEmailAsync(user.Email);

                    if (!await _userManager.IsInRoleAsync(user, Role.ADMINISTRATOR_ROLE))
                        result = await _userManager.AddToRoleAsync(user, Role.ADMINISTRATOR_ROLE);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation($"{Role.ADMINISTRATOR_ROLE} role has been assigned to user {@User}.", user);

                    // update or create core settings
                    var settings = await settingService.GetSettingsAsync<CoreSettings>();
                    if (settings != null)
                    {
                        settings.Title = model.Title;
                        settings.TimeZoneId = model.TimeZoneId;
                        settings.SetupDone = true;
                        await settingService.UpsertSettingsAsync(settings);
                    }
                    else
                    {
                        await settingService.UpsertSettingsAsync(new CoreSettings
                        {
                            Title = model.Title,
                            TimeZoneId = model.TimeZoneId,
                            SetupDone = true,
                        });
                    }
                    _logger.LogInformation("CoreSettings created");

                    // sign-in user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User has been signed in");

                    // setup blog
                    await SetupBlogSettingsAndPostsAsync();
                    await SetupPagesAndNavigationAsync();
                    await SetupThemePluginsAndWidgetsAsync();
                    _logger.LogInformation("Blog setup completes");

                    return new JsonResult(true);
                }

                return BadRequest(result.Errors.ToList()[0].Description);
            }
            catch (MokException ex)
            {
                return BadRequest(ex.ValidationErrors[0].ErrorMessage);
            }
        }

        /// <summary>
        /// Creates pre-defined system roles.
        /// </summary>
        private async Task<IdentityResult> CreateSystemRolesAsync()
        {
            IdentityResult result = IdentityResult.Success;

            // Administrator role
            if (!await _roleManager.RoleExistsAsync(Role.ADMINISTRATOR_ROLE))
            {
                result = await _roleManager.CreateAsync(new Role
                {
                    Name = Role.ADMINISTRATOR_ROLE,
                    IsSystemRole = true,
                    Description = "Administrator has full power over the site and can do everything."
                });
                _logger.LogInformation($"{Role.ADMINISTRATOR_ROLE} role created.");
            }

            // Editor role
            if (!await _roleManager.RoleExistsAsync(Role.EDITOR_ROLE))
            {
                result = await _roleManager.CreateAsync(new Role
                {
                    Name = Role.EDITOR_ROLE,
                    IsSystemRole = true,
                    Description = "Editor can only publish and manage posts including the posts of other users."
                });
                _logger.LogInformation($"{Role.EDITOR_ROLE} role created.");
            }

            return result;
        }

        /// <summary>
        /// Creates the settings, tags and the default category.
        /// </summary>
        private async Task SetupBlogSettingsAndPostsAsync()
        {
            var blogSettings = await settingService.GetSettingsAsync<BlogSettings>(); // could be initial or an existing blogsettings
            await settingService.UpsertSettingsAsync(blogSettings);
            _logger.LogInformation("Blog settings created");

            const string DEFAULT_CATEGORY = "Software Development";

            // get default cat
            Category defaultCat;
            try
            {
                defaultCat = await _catSvc.GetAsync(blogSettings.DefaultCategoryId);
            }
            catch (MokException)
            {
                defaultCat = await _catSvc.CreateAsync(DEFAULT_CATEGORY);
            }

            _logger.LogInformation("Blog categories created");
        }

        /// <summary>
        /// Creates default pages and setup navigation.
        /// </summary>
        /// <returns></returns>
        private async Task SetupPagesAndNavigationAsync()
        {
            // "about"
            var aboutPage = await pageService.CreateAsync(new Blog.Models.Page
            {
                UserId = 1,
                Title = "About",
                Status = EPostStatus.Published,
                CreatedOn = DateTimeOffset.Now,
                PageLayout = (byte)EPageLayout.Layout1, // default
                Excerpt = "About the Mok project.",
                Body = await GetSetupFileContent("page-about.html"),
                BodyMark = await GetSetupFileContent("page-about.md"),
            });

            _logger.LogInformation("Default pages created");

            // Blog (App)
            await navigationService.AddNavToMenuAsync(EMenu.Menu1, 0, new Nav
            {
                Id = App.BLOG_APP_ID,
                Text = App.BLOG_APP_NAME,
                Type = ENavType.App
            });


            // About (Page)
            await navigationService.AddNavToMenuAsync(EMenu.Menu1, 2, new Nav
            {
                Id = aboutPage.Id,
                Text = aboutPage.Title,
                Type = ENavType.Page
            });

            _logger.LogInformation("Site navigation created");
        }

        /// <summary>
        /// Returns the setup file content.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<string> GetSetupFileContent(string fileName)
        {
            var setupPath = Path.Combine(hostingEnvironment.ContentRootPath, SETUP_DATA_DIR);
            var filePath = Path.Combine(setupPath, fileName);
            return await System.IO.File.ReadAllTextAsync(filePath);
        }
    }

    public class SetupValidator : AbstractValidator<SetupModel>
    {
        /// <summary>
        /// DisplayName or UserName should be at least 2 chars min.
        /// </summary>
        public const int NAME_MINLENGTH = 2;
        /// <summary>
        /// UserName should be no more than 20 chars max.
        /// </summary>
        public const int USERNAME_MAXLENGTH = 20;
        /// <summary>
        /// DisplayName should be no more than 32 chars max.
        /// </summary>
        public const int DISPLAYNAME_MAXLENGTH = 32;
        /// <summary>
        /// Password should be at least 8 chars min.
        /// </summary>
        public const int PASSWORD_MINLENGTH = 8;
        /// <summary>
        /// UserName can only contain alphanumeric, dash and underscore.
        /// </summary>
        public const string USERNAME_REGEX = @"^[a-zA-Z0-9-_]+$";

        public SetupValidator()
        {
            // Email
            RuleFor(s => s.Email).EmailAddress();

            // UserName
            RuleFor(s => s.UserName)
                .NotEmpty()
                .Length(NAME_MINLENGTH, USERNAME_MAXLENGTH)
                .Matches(USERNAME_REGEX)
                .WithMessage(s => $"Username '{s.UserName}' is not available.");

            // DisplayName
            RuleFor(s => s.DisplayName)
                .NotEmpty()
                .Length(NAME_MINLENGTH, DISPLAYNAME_MAXLENGTH);

            // Password
            RuleFor(s => s.Password).MinimumLength(PASSWORD_MINLENGTH);
        }
    }
}
