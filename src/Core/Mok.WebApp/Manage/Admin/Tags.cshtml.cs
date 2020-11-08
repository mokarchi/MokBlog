using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Newtonsoft.Json;

namespace Mok.WebApp.Manage.Admin
{
    public class TagsModel : PageModel
    {
        private readonly ITagService _tagSvc;

        public TagsModel(ITagService tagService)
        {
            _tagSvc = tagService;
        }

        public string TagListJsonStr { get; private set; }

        /// <summary>
        /// GET bootstrap page with json data.
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            var tags = await _tagSvc.GetAllAsync();
            TagListJsonStr = JsonConvert.SerializeObject(tags);
        }

        /// <summary>
        /// POST to create a new tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync([FromBody] Tag tag)
        {
            try
            {
                var tagNew = await _tagSvc.CreateAsync(new Tag { Title = tag.Title, Description = tag.Description });
                return new JsonResult(tagNew);
            }
            catch (MokException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// POST to udpate an existing tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateAsync([FromBody] Tag tag)
        {
            try
            {
                var tagUpdated = await _tagSvc.UpdateAsync(tag);
                return new JsonResult(tagUpdated);
            }
            catch (MokException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// DELETE a tag by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task OnDeleteAsync(int id)
        {
            await _tagSvc.DeleteAsync(id);
        }
    }
}
