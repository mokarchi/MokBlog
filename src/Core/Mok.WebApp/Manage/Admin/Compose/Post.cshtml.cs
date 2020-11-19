using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Services.Interfaces;
using Mok.Medias;
using Mok.Membership;
using Mok.Settings;

namespace Mok.WebApp.Manage.Admin.Compose
{
    /// <summary>
    /// Post composer.
    /// </summary>
    public class PostModel : PageModel
    {
        /// <summary>
        /// How many seconds to wait after user stops typing to auto save. Default 10 seconds.
        /// </summary>
        public const int AUTOSAVE_INTERVAL = 10;
        /// <summary>
        /// Post date display format.
        /// </summary>
        /// <remarks>
        /// Vuetify datepicker works this format by default and it's a lot more work to change this format.
        /// </remarks>
        private const string DATE_FORMAT = "yyyy-MM-dd";
        public string PostJson { get; set; }
        public string CatsJson { get; set; }
        public string TagsJson { get; set; }
        public string Theme { get; set; }

        private readonly IBlogPostService _blogSvc;
        private readonly ICategoryService _catSvc;
        private readonly ITagService _tagSvc;
        private readonly ISettingService _settingSvc;
        private readonly UserManager<User> _userManager;
        private readonly IMediaService _mediaSvc;

        public PostModel(
            UserManager<User> userManager,
            IBlogPostService blogService,
            ICategoryService catService,
            ITagService tagService,
            IMediaService mediaSvc,
            ISettingService settingService)
        {
            _userManager = userManager;
            _blogSvc = blogService;
            _catSvc = catService;
            _tagSvc = tagService;
            _mediaSvc = mediaSvc;
            _settingSvc = settingService;
        }

        public void OnGet()
        {
        }
    }
}
