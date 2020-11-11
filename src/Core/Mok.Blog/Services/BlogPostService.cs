using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Enums;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IDistributedCache cache;
        private readonly ILogger<BlogPostService> logger;

        public BlogPostService(IPostRepository postRepository, 
                               IDistributedCache cache,
                               ILogger<BlogPostService> logger)
        {
            this.postRepository = postRepository;
            this.cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// Returns a list of blog posts.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        /// <remarks>
        /// For the blog index page, admin post page, main rss feed.
        /// </remarks>
        public async Task<BlogPostList> GetListAsync(int pageIndex, int pageSize, bool cacheable = true)
        {
            PostListQuery query = new PostListQuery(EPostListQueryType.BlogPosts)
            {
                PageIndex = (pageIndex <= 0) ? 1 : pageIndex,
                PageSize = pageSize,
            };

            // cache only first page of the public site, not admin or rss
            if (query.PageIndex == 1 && cacheable)
            {
                return await cache.GetAsync(BlogCache.KEY_POSTS_INDEX, BlogCache.Time_Posts_Index, async () =>
                {
                    return await QueryPostsAsync(query);
                });
            }

            return await QueryPostsAsync(query);
        }

        /// <summary>
        /// Returns a <see cref="BlogPostList"/> based on query from data source.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private async Task<BlogPostList> QueryPostsAsync(PostListQuery query)
        {
            var (posts, totalCount) = await postRepository.GetListAsync(query);

            var blogPostList = new BlogPostList
            {
                TotalPostCount = totalCount
            };
            foreach (var post in posts)
            {
                var blogPost = ConvertToBlogPost(post);
                blogPost = await PreRenderAsync(blogPost);
                blogPostList.Posts.Add(blogPost);
            }

            return blogPostList;
        }
    }
}
