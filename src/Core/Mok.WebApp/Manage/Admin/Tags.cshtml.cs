using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Models;

namespace Mok.WebApp.Manage.Admin
{
    public class TagsModel : PageModel
    {
        public void OnGet()
        {
        }

        public string TagListJsonStr { get; private set; }

        /// <summary>
        /// GE Tbootstrap page with json data.
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {

        }

        /// <summary>
        /// POST to create a new tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync([FromBody] Tag tag)
        {
            throw new Exception();
        }

        /// <summary>
        /// POST to udpate an existing tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateAsync([FromBody] Tag tag)
        {
            throw new Exception();
        }

        /// <summary>
        /// DELETE a tag by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task OnDeleteAsync(int id)
        {

        }
    }
}
