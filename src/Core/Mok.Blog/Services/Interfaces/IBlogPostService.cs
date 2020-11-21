using Mok.Blog.Models;
using System.Threading.Tasks;

namespace Mok.Blog.Services.Interfaces
{
    public interface IBlogPostService
    {
        /// <summary>
        /// Returns <see cref="BlogPostList"/> for blog main index.
        /// </summary>
        /// <param name="pageIndex">Pagination 1-based</param>
        Task<BlogPostList> GetListAsync(int pageIndex, int pageSize, bool cacheable = true);

        /// <summary>
        /// Returns all blog post drafts.
        /// </summary>
        /// <returns></returns>
        Task<BlogPostList> GetListForDraftsAsync();

        /// <summary>
        /// Deletes a <see cref="BlogPost"/> by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Returns a <see cref="BlogPost"/> by id, throws <see cref="FanException"/> if not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="FanException">if post is not found.</exception>
        Task<BlogPost> GetAsync(int id);
    }
}
