using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Services.Interfaces;
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
    }
}
