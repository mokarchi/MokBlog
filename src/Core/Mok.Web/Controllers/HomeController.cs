using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public async Task<IActionResult> Index(int? page)
        {
            return View();
        }
    }
}
