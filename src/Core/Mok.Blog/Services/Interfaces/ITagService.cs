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

        /// <summary>
        /// Creates a new <see cref="Tag"/>.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task<Tag> CreateAsync(Tag tag);
    }
}
