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
    }
}
