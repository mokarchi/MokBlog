using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Mok.Settings;
using Newtonsoft.Json;

namespace Mok.WebApp.Manage.Admin
{
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService _catSvc;
        private readonly ISettingService _settingSvc;

        public CategoriesModel(ICategoryService catService,
                               ISettingService settingService)
        {
            _catSvc = catService;
            _settingSvc = settingService;
        }

        public string CategoryListJsonStr { get; private set; }
        public int DefaultCategoryId { get; private set; }

        /// <summary>
        /// GET
        /// </summary>
        public async Task OnGetAsync()
        {
            var blogSettings = await _settingSvc.GetSettingsAsync<BlogSettings>();
            DefaultCategoryId = blogSettings.DefaultCategoryId;

            var cat = await _catSvc.GetAllAsync();
            CategoryListJsonStr = JsonConvert.SerializeObject(cat);
        }

        /// <summary>
        /// POST to create a new category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync([FromBody]Category category)
        {
            try
            {
                var cat = await _catSvc.CreateAsync(category.Title,category.Description);
                return new JsonResult(cat);
            }
            catch (MokException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// DELETE a category by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task OnDeleteAsync(int id)
        {
            await _catSvc.DeleteAsync(id);
        }

        /// <summary>
        /// POST to udpate an existing category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateAsync([FromBody]Category category)
        {
            try
            {
                var cat = await _catSvc.UpdateAsync(category);
                return new JsonResult(cat);
            }
            catch (MokException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// POST to set default category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task OnPostDefaultAsync(int id)
        {
            await _catSvc.SetDefaultAsync(id);
        }
    }
}
