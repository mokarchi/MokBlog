using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mok.WebApp.Manage.Admin
{
    /// <summary>
    /// The admin index page, it redirects.
    /// </summary>
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect($"admin/posts");
        }
    }
}
