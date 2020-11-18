using Mok.Blog.Enums;
using Mok.Blog.Models;
using Mok.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Blog.Data
{
    /// <summary>
    /// Contract for a post repository.
    /// </summary>
    /// <remarks>
    /// Data operations for both blog posts and pages.
    /// </remarks>
    public interface IPostRepository : IRepository<Post>
    {
        /// <summary>
        /// Returns a list of posts and total post count by query or empty list if no posts found.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<(IList<Post> posts, int totalCount)> GetListAsync(PostListQuery query);

        /// <summary>
        /// Returns a <see cref="Post"/> by id. If it is a BlogPost it'll return together with its 
        /// <see cref="Category"/> and <see cref="Tag"/>. Returns null if it's not found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">If it's BlogPost it'll return category and tags with it.</param>
        Task<Post> GetAsync(int id, EPostType type);
    }
}
