using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Mok.Blog.Services.Interfaces;
using Mok.Membership;
using Mok.Settings;
using Newtonsoft.Json;

namespace Mok.WebApp.Manage
{
    public class SetupModel : PageModel
    {
        private readonly ISettingService settingService;

        public const string SETUP_DATA_DIR = "Setup";

        public SetupModel(ISettingService settingService)
        {
            this.settingService = settingService;
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
    }
}
