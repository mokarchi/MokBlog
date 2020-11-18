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
    }
}
