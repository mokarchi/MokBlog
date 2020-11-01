using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Newtonsoft.Json;

namespace Mok.WebApp.Manage.Admin
{
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService _catSvc;

        public CategoriesModel(ICategoryService catService)
        {
            _catSvc = catService;
        }

        public string CategoryListJsonStr { get; private set; }

        /// <summary>
        /// GET
        /// </summary>
        public async Task OnGetAsync()
        {
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
    }
}
