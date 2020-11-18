using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Enums;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using System.Net;
using System.Threading.Tasks;
using Mok.Helpers;
using System;
using Mok.Exceptions;

namespace Mok.Blog.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IDistributedCache cache;
        private readonly ILogger<BlogPostService> logger;
        private readonly IMapper mapper;
        private readonly IImageService imageService;

        public BlogPostService(IPostRepository postRepository, 
                               IDistributedCache cache,
                               ILogger<BlogPostService> logger,
                               IImageService imageService,
                               IMapper mapper)
        {
            this.postRepository = postRepository;
            this.imageService = imageService;
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// How many words to extract into excerpt from body. Default 55.
        /// </summary>
        public const int EXCERPT_WORD_LIMIT = 55;

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
        /// Returns a list of blog drafts.
        /// </summary>
        /// <returns></returns>
        public async Task<BlogPostList> GetListForDraftsAsync()
        {
            PostListQuery query = new PostListQuery(EPostListQueryType.BlogDrafts);

            return await QueryPostsAsync(query);
        }

        /// <summary>
        /// Deletes a blog post and invalidates cache for posts on index page.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var post = await GetAsync(id);
            await postRepository.DeleteAsync(id);
            await RemoveBlogCacheAsync();
            await RemoveSinglePostCacheAsync(post);
        }

        /// <summary>
        /// Returns a <see cref="BlogPost"/> by id with its <see cref="Category"/> and <see cref="Tag"/>
        /// and throws <see cref="FanException"/> if not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="FanException">if post is not found.</exception>
        /// <remarks>
        /// This is used when you want to get a post from db.
        /// </remarks>
        public async Task<BlogPost> GetAsync(int id)
        {
            var post = await QueryPostAsync(id);
            return ConvertToBlogPost(post);
        }

        /// <summary>
        /// Returns a <see cref="Post"/> from data source, throws <see cref="FanException"/> if not found.
        /// </summary>
        /// <param name="id">A blog post id.</param>
        /// <returns></returns>
        /// <remarks>
        /// Returned post is tracked.
        /// </remarks>
        private async Task<Post> QueryPostAsync(int id)
        {
            var post = await postRepository.GetAsync(id, EPostType.BlogPost);

            if (post == null)
            {
                throw new MokException($"Blog post with id {id} is not found.");
            }

            return post;
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

        /// <summary>
        /// Gets a <see cref="BlogPost"/> for display to client from a <see cref="Post"/>.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        /// <remarks>
        /// It readies <see cref="Post.CreatedOnDisplay"/>, Title, Excerpt, CategoryTitle, Tags and Body with shortcodes.
        /// </remarks>
        private BlogPost ConvertToBlogPost(Post post)
        {
            var blogPost = mapper.Map<Post, BlogPost>(post);

            // Title
            blogPost.Title = WebUtility.HtmlDecode(blogPost.Title); // since OLW encodes it, we decode it here

            // Excerpt
            blogPost.Excerpt = post.Excerpt.IsNullOrEmpty() ? Util.GetExcerpt(post.Body, EXCERPT_WORD_LIMIT) : post.Excerpt;

            // CategoryTitle
            blogPost.CategoryTitle = post.Category?.Title;

            // Tags and TagTitles
            foreach (var postTag in post.PostTags)
            {
                blogPost.Tags.Add(postTag.Tag);
                blogPost.TagTitles.Add(postTag.Tag.Title);
            }

            // ViewCount
            blogPost.ViewCount = post.ViewCount;

            logger.LogDebug("Show {@BlogPost}", blogPost);
            return blogPost;
        }

        /// <summary>
        /// Pre render processing of a blog post. TODO consider refactor.
        /// </summary>
        /// <param name="blogPost"></param>
        /// <returns></returns>
        private async Task<BlogPost> PreRenderAsync(BlogPost blogPost)
        {
            if (blogPost == null) return blogPost;

            blogPost.Body = OembedParser.Parse(blogPost.Body);
            blogPost.Body = await imageService.ProcessResponsiveImageAsync(blogPost.Body);

            return blogPost;
        }

        /// <summary>
        /// Invalidates cache for blog post service.
        /// </summary>
        public async Task RemoveBlogCacheAsync()
        {
            await cache.RemoveAsync(BlogCache.KEY_POSTS_INDEX);
            await cache.RemoveAsync(BlogCache.KEY_POSTS_RECENT);
            await cache.RemoveAsync(BlogCache.KEY_ALL_CATS);
            await cache.RemoveAsync(BlogCache.KEY_ALL_TAGS);
            await cache.RemoveAsync(BlogCache.KEY_ALL_ARCHIVES);
            await cache.RemoveAsync(BlogCache.KEY_POST_COUNT);
        }

        private async Task RemoveSinglePostCacheAsync(Post post)
        {
            var cacheKey = string.Format(BlogCache.KEY_POST, post.Slug, post.CreatedOn.Year, post.CreatedOn.Month, post.CreatedOn.Day);
            await cache.RemoveAsync(cacheKey);
        }
    }
}
