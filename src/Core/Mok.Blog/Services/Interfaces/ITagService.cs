using Mok.Blog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mok.Blog.Services.Interfaces
{
    public interface ITagService
    {
        /// <summary>
        /// Returns all tags, cached after calls to DAL.
        /// </summary>
        /// <returns></returns>
        Task<List<Tag>> GetAllAsync();
    }
}
