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
        /// Creates a <see cref="Post"/>.
        /// </summary>
        /// <param name="post">The post to create.</param>
        /// <param name="categoryTitle">The category title is available when called from metaweblog.</param>
        /// <param name="tagTitles">A list of tag titles associated with the post.</param>
        /// <returns>
        /// The inserted post with id.
        /// </returns>
        Task<Post> CreateAsync(Post post, string categoryTitle, IEnumerable<string> tagTitles);

        /// <summary>
        /// Updates a <see cref="Post"/>.
        /// </summary>
        /// <param name="post">The post to update.</param>
        /// <param name="categoryTitle">The category title of the blog post input.</param>
        /// <param name="tagTitles">A list of tag titles associated with the post.</param>
        Task UpdateAsync(Post post, string categoryTitle, IEnumerable<string> tagTitles);

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

        /// <summary>
        /// Returns a <see cref="EPostStatus.Published"/> <see cref="Post"/>, returns null if it's not found.
        /// </summary>
        Task<Post> GetAsync(string slug, int year, int month, int day);
    }
}
