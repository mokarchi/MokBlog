using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mok.WebApp.Manage.Admin
{
    public class MediaModel : PageModel
    {
        /// <summary>
        /// Display 96 images at a time.
        /// </summary>
        public const int PAGE_SIZE = 96;

        public void OnGet()
        {
        }
    }
}
