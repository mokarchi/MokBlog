using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Enums;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Models.Input;
using Mok.Blog.Services.Interfaces;
using Mok.Medias;
using Mok.Settings;
using Newtonsoft.Json;

namespace Mok.WebApp.Manage.Admin.Compose
{
    /// <summary>
    /// Post composer.
    /// </summary>
    public class PostModel : PageModel
    {
        private readonly IBlogPostService _blogSvc;
        private readonly ICategoryService _catSvc;
        private readonly ITagService _tagSvc;
        private readonly ISettingService _settingSvc;
        private readonly IMediaService _mediaSvc;

        public PostModel(
            IBlogPostService blogService,
            ICategoryService catService,
            ITagService tagService,
            IMediaService mediaSvc,
            ISettingService settingService)
        {
            _blogSvc = blogService;
            _catSvc = catService;
            _tagSvc = tagService;
            _mediaSvc = mediaSvc;
            _settingSvc = settingService;
        }

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

        /// <summary>
        /// GET to return <see cref="BlogPostIM"/> to initialize the page.
        /// </summary>
        /// <remarks>
        /// NOTE: the parameter cannot be named "page".
        /// </remarks>
        /// <param name="postId">0 for a new post or an existing post id</param>
        /// <returns></returns>
        public async Task OnGetAsync(int postId)
        {
            // theme
            var coreSettings = await _settingSvc.GetSettingsAsync<CoreSettings>();
            Theme = coreSettings.Theme;

            // post
            BlogPostIM postIM;
            if (postId > 0) // existing post
            {
                var post = await _blogSvc.GetAsync(postId);
                var postDate = post.CreatedOn.ToLocalTime(coreSettings.TimeZoneId).ToString(DATE_FORMAT);

                postIM = new BlogPostIM
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    PostDate = postDate,
                    Slug = post.Slug,
                    Excerpt = post.Excerpt,
                    CategoryId = post.CategoryId ?? 1,
                    Tags = post.TagTitles,
                    Published = post.Status == EPostStatus.Published,
                    IsDraft = post.Status == EPostStatus.Draft,
                    DraftDate = post.UpdatedOn.HasValue ? post.UpdatedOn.Value.ToString(DATE_FORMAT) : "",
                };
            }
            else // new post
            {
                var blogSettings = await _settingSvc.GetSettingsAsync<BlogSettings>();
                var postDate = DateTimeOffset.UtcNow.ToLocalTime(coreSettings.TimeZoneId).ToString(DATE_FORMAT);

                postIM = new BlogPostIM
                {
                    Title = "",
                    Body = "",
                    PostDate = postDate,
                    CategoryId = blogSettings.DefaultCategoryId,
                    Tags = new List<string>(),
                    Published = false,
                    IsDraft = false,
                };
            }
            PostJson = JsonConvert.SerializeObject(postIM);

            // cats
            var categories = await _catSvc.GetAllAsync();
            var allCats = from c in categories
                          select new
                          {
                              Value = c.Id,
                              Text = c.Title,
                          };
            CatsJson = JsonConvert.SerializeObject(allCats);

            // tags
            var tags = await _tagSvc.GetAllAsync();
            var allTags = tags.Select(t => t.Title).ToArray();
            TagsJson = JsonConvert.SerializeObject(allTags);
        }

        /// <summary>
        /// Ajax POST to save a post as draft.
        /// </summary>
        /// <returns>
        /// The updated <see cref="BlogPost"/>.
        /// </returns>
        /// <remarks>
        /// This is called by either auto save or user clicking on Save.
        /// </remarks>
        public async Task<JsonResult> OnPostSaveAsync([FromBody] BlogPostIM postIM)
        {
            var blogPost = new BlogPost
            {
                CategoryId = postIM.CategoryId,
                CreatedOn = BlogUtil.GetCreatedOn(postIM.PostDate),
                TagTitles = postIM.Tags,
                Slug = postIM.Slug,
                Excerpt = postIM.Excerpt,
                Title = postIM.Title,
                Body = postIM.Body,
                Status = EPostStatus.Draft,
            };

            if (postIM.Id <= 0)
            {
                blogPost = await _blogSvc.CreateAsync(blogPost);
            }
            else
            {
                blogPost.Id = postIM.Id;
                blogPost = await _blogSvc.UpdateAsync(blogPost);
            }

            var coreSettings = await _settingSvc.GetSettingsAsync<CoreSettings>();

            var postVM = new BlogPostIM
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Body = blogPost.Body,
                PostDate = blogPost.CreatedOn.ToString(DATE_FORMAT),
                Slug = blogPost.Slug,
                Excerpt = blogPost.Excerpt,
                CategoryId = blogPost.CategoryId ?? 1,
                Tags = blogPost.TagTitles,
                Published = blogPost.Status == EPostStatus.Published,
                IsDraft = blogPost.Status == EPostStatus.Draft,
                DraftDate = blogPost.UpdatedOn.HasValue ? blogPost.UpdatedOn.Value.ToDisplayString(coreSettings.TimeZoneId) : "",
            };

            return new JsonResult(postVM);
        }
    }
}
