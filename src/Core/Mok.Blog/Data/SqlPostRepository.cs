using Microsoft.EntityFrameworkCore;
using Mok.Blog.Enums;
using Mok.Blog.Models;
using Mok.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Blog.Data
{
    /// <summary>
    /// Sql implementation of the <see cref="IPostRepository"/> contract.
    /// </summary>
    public class SqlPostRepository : EntityRepository<Post>, IPostRepository
    {
        private readonly ApplicationDbContext _db;
        public SqlPostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a <see cref="Post"/>.
        /// </summary>
        /// <param name="post">The post to create.</param>
        /// <param name="categoryTitle">The category title is available when called from metaweblog.</param>
        /// <param name="tagTitles">A list of tag titles associated with the post.</param>
        /// <returns>
        /// The inserted post with id.
        /// </returns>
        public async Task<Post> CreateAsync(Post post, string categoryTitle, IEnumerable<string> tagTitles)
        {
            // Category
            if (!categoryTitle.IsNullOrEmpty())
            {
                // cat title present, olw and setup
                post.Category = _db.Set<Category>().First(c => c.Title.ToUpper() == categoryTitle.ToUpper());
            }
            else if (!post.CategoryId.HasValue) // from browser CategoryId will have value
            {
                // from metaweblog with no cat inputted, give it the default cat id
                post.CategoryId =
                    Convert.ToInt32(_db.Set<Meta>().First(m => m.Key.Equals("blogsettings.defaultcategoryid")).Value);
            }

            // PostTags
            if (!tagTitles.IsNullOrEmpty())
            {
                // make sure list has no empty strings and only unique values
                tagTitles = tagTitles.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

                var tags = _db.Set<Tag>(); // all tags
                foreach (var title in tagTitles)
                {
                    // lookup the tag (any new tag is already created prior)
                    var tag = tags.First(t => t.Title.ToUpper() == title.ToUpper());
                    post.PostTags.Add(new PostTag { Post = post, Tag = tag });
                }
            }

            await _entities.AddAsync(post);
            await _db.SaveChangesAsync();
            return post;
        }

        /// <summary>
        /// Updates a <see cref="Post"/>.
        /// </summary>
        /// <param name="post">The post to update.</param>
        /// <param name="categoryTitle">The category title of the blog post input.</param>
        /// <param name="tagTitles">A list of tag titles associated with the post.</param>
        public async Task UpdateAsync(Post post, string categoryTitle, IEnumerable<string> tagTitles)
        {
            // Category
            if (!categoryTitle.IsNullOrEmpty()) // if cat title has value
            {
                // from metaweblog with a cat inputted
                post.Category = _db.Set<Category>().First(c => c.Title.ToUpper() == categoryTitle.ToUpper());
                post.CategoryId = post.Category.Id;
            }

            // PostTags
            if (!tagTitles.IsNullOrEmpty())
            {
                // make sure list has no empty strings and only unique values, olw passes empty string when no tags are given
                tagTitles = tagTitles.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

                var currentTitles = post.PostTags.Select(pt => pt.Tag.Title);
                var titlesToRemove = currentTitles.Except(tagTitles).ToList();
                foreach (var title in titlesToRemove)
                {
                    post.PostTags.Remove(post.PostTags.Single(pt => pt.Tag.Title == title));
                }

                var tags = _db.Set<Tag>(); // all tags
                var titlesToAdd = tagTitles.Except(currentTitles);
                foreach (var title in titlesToAdd)
                {
                    var tag = tags.First(t => t.Title.ToUpper() == title.ToUpper());
                    post.PostTags.Add(new PostTag { PostId = post.Id, TagId = tag.Id });
                }
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns a list of posts and total post count by query or empty list if no posts found.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<(IList<Post> posts, int totalCount)> GetListAsync(PostListQuery query)
        {
            IList<Post> posts = null;
            int skip = (query.PageIndex - 1) * query.PageSize;
            int take = query.PageSize;
            IQueryable<Post> q = (query.QueryType == EPostListQueryType.Pages || query.QueryType == EPostListQueryType.PagesWithChildren) ?
                _entities.Include(p => p.User).Where(p => p.Type == EPostType.Page) :
                _entities.Include(p => p.User).Include(p => p.Category).Include(p => p.PostTags).ThenInclude(p => p.Tag).Where(p => p.Type == EPostType.BlogPost);

            switch (query.QueryType)
            {
                case EPostListQueryType.BlogPosts:
                    q = q.Where(p => p.Status == EPostStatus.Published);
                    posts = await q.OrderByDescending(p => p.CreatedOn).Skip(skip).Take(take).ToListAsync();
                    break;
                case EPostListQueryType.BlogDrafts:
                    q = q.Where(p => p.Status == EPostStatus.Draft);
                    posts = await q.OrderByDescending(p => p.UpdatedOn).ToListAsync();
                    break;
                case EPostListQueryType.BlogPostsByCategory:
                    var cat = await _db.Set<Category>().FirstAsync(t => t.Slug == query.CategorySlug);
                    q = q.Where(p => p.CategoryId == cat.Id && p.Status == EPostStatus.Published);
                    posts = await q.OrderByDescending(p => p.CreatedOn).Skip(skip).Take(take).ToListAsync();
                    break;
                case EPostListQueryType.BlogPostsByTag:
                    var tag = await _db.Set<Tag>().FirstAsync(t => t.Slug == query.TagSlug);
                    q = from p in q
                        from pt in p.PostTags
                        where p.Id == pt.PostId &&
                        pt.TagId == tag.Id && p.Status == EPostStatus.Published
                        select p;
                    posts = await q.OrderByDescending(p => p.CreatedOn).Skip(skip).Take(take).ToListAsync();
                    break;
                case EPostListQueryType.BlogPostsArchive:
                    q = (query.Month.HasValue && query.Month > 0) ?
                        q.Where(p => p.CreatedOn.Year == query.Year && p.CreatedOn.Month == query.Month && p.Status == EPostStatus.Published) :
                        q.Where(p => p.CreatedOn.Year == query.Year && p.Status == EPostStatus.Published);
                    posts = await q.OrderByDescending(p => p.CreatedOn).ToListAsync();
                    break;
                case EPostListQueryType.BlogPostsByNumber:
                    posts = await q.OrderByDescending(p => p.CreatedOn).Take(take).ToListAsync();
                    break;
                case EPostListQueryType.BlogPublishedPostsByNumber:
                    q = q.Where(p => p.Status == EPostStatus.Published);
                    posts = await q.OrderByDescending(p => p.CreatedOn).Take(take).ToListAsync();
                    break;
                case EPostListQueryType.Pages:
                    q = q.Where(p => p.ParentId == null || p.ParentId == 0);
                    posts = await q.OrderByDescending(p => p.CreatedOn).ToListAsync();
                    break;
                case EPostListQueryType.PagesWithChildren:
                    posts = await q.OrderByDescending(p => p.CreatedOn).ToListAsync();
                    break;
            }

            int postCount = await q.CountAsync();

            return (posts, totalCount: postCount);
        }

        /// <summary>
        /// Returns a <see cref="Post"/> by id. If it is a BlogPost it'll return together with its 
        /// <see cref="Category"/> and <see cref="Tag"/>. Returns null if it's not found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">If it's BlogPost it'll return category and tags with it.</param>
        public async Task<Post> GetAsync(int id, EPostType type)
        {
            return (type == EPostType.BlogPost) ?
                await _entities.Include(p => p.User).Include(p => p.Category).Include(p => p.PostTags).ThenInclude(p => p.Tag).SingleOrDefaultAsync(p => p.Id == id) :
                await _entities.Include(p => p.User).SingleOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Returns a <see cref="EPostStatus.Published"/> <see cref="Post"/>, returns null if it's not found.
        /// </summary>
        public async Task<Post> GetAsync(string slug, int year, int month, int day) =>
            isSqlite ?
                 _entities.Include(p => p.User).Include(p => p.Category).Include(p => p.PostTags).ThenInclude(p => p.Tag).ToList()
                                   .SingleOrDefault(p =>
                                     p.Type == EPostType.BlogPost &&
                                     p.Status == EPostStatus.Published &&
                                     p.Slug.ToUpper() == slug.ToUpper() &&
                                     p.CreatedOn.Year == year &&
                                     p.CreatedOn.Month == month &&
                                     p.CreatedOn.Day == day) :
                 await _entities.Include(p => p.User).Include(p => p.Category).Include(p => p.PostTags).ThenInclude(p => p.Tag)
                               .SingleOrDefaultAsync(p =>
                                 p.Type == EPostType.BlogPost &&
                                 p.Status == EPostStatus.Published &&
                                 p.Slug.ToUpper() == slug.ToUpper() &&
                                 p.CreatedOn.Year == year &&
                                 p.CreatedOn.Month == month &&
                                 p.CreatedOn.Day == day);

    }
}
