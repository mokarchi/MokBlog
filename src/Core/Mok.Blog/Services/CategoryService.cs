using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Mok.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ILogger<CategoryService> logger;
        private readonly IDistributedCache cache;
        public CategoryService(ICategoryRepository categoryRepository,
                               IDistributedCache cache,
                               ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// The max allowed length of a category title is 24 chars.
        /// </summary>
        public const int TITLE_MAXLEN = 24;

        /// <summary>
        /// The max allowed length of a category slug is 24 chars.
        /// </summary>
        public const int SLUG_MAXLEN = 24;

        /// <summary>
        /// Returns all categories, cached after calls to DAL.
        /// </summary>
        public async Task<List<Category>> GetAllAsync()
        {
            return await cache.GetAsync(BlogCache.KEY_ALL_CATS, BlogCache.Time_AllCats, async () =>
            {
                return await categoryRepository.GetListAsync();
            });
        }

        /// <summary>
        /// Returns category by id, throws <see cref="MokException"/> if category with id is not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Category> GetAsync(int id)
        {
            var cats = await GetAllAsync();
            var cat = cats.SingleOrDefault(c => c.Id == id);
            if (cat == null)
            {
                throw new MokException(EExceptionType.ResourceNotFound,
                    $"Category with id {id} is not found.");
            }

            return cat;
        }

        /// <summary>
        /// Creates a new <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The category with data to be created.</param>
        /// <exception cref="MokException">If title is empty or exists already.</exception>
        /// <returns>Created category.</returns>
        public async Task<Category> CreateAsync(string title, string description = null)
        {
            if (title.IsNullOrEmpty())
            {
                throw new MokException($"Category title cannot be empty.");
            }

            title = PrepareTitle(title);

            // make sure unique
            var allCats = await GetAllAsync();
            if(allCats.Any(t => t.Title.Equals(title,StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new MokException($"'{title}' already exists.");
            }

            var category = new Category
            {
                Title = title,
                Slug = BlogUtil.SlugifyTaxonomy(title, SLUG_MAXLEN, allCats.Select(c => c.Slug)),
                Description = Util.CleanHtml(description),
                Count = 0
            };

            // create
            category = await categoryRepository.CreateAsync(category);

            // remove cache
            await cache.RemoveAsync(BlogCache.KEY_ALL_CATS);

            logger.LogDebug("Created {@Category}", category);
            return category;
        }

        public async Task DeleteAsync(int id)
        {
            // delete
            await categoryRepository.DeleteAsync(id, 1);

            // invalidate cache
            await cache.RemoveAsync(BlogCache.KEY_ALL_CATS);
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            if(category == null || category.Id <=0 || category.Title.IsNullOrEmpty())
            {
                throw new MokException($"Invalid category to update.");
            }

            category.Title = PrepareTitle(category.Title);

            // make sure it is unique
            var allCats = await GetAllAsync();
            allCats.RemoveAll(c => c.Id == category.Id);
            if (allCats.Any(c => c.Title.Equals(category.Title, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new MokException($"'{category.Title}' already exists.");
            }

            var entity = await categoryRepository.GetAsync(category.Id);
            entity.Title = category.Title;
            entity.Slug = BlogUtil.SlugifyTaxonomy(category.Title, SLUG_MAXLEN, allCats.Select(c => c.Slug));
            entity.Description = Util.CleanHtml(category.Description);
            entity.Count = category.Count;

            await categoryRepository.UpdateAsync(category);

            // remove cache
            await cache.RemoveAsync(BlogCache.KEY_ALL_CATS);

            // return entity
            logger.LogDebug("Updated {@Category}", entity);
            return entity;
        }

        /// <summary>
        /// Cleans category title from any html and shortens it if exceed max allow length.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string PrepareTitle(string title)
        {
            title = Util.CleanHtml(title);
            title = title.Length > TITLE_MAXLEN ? title.Substring(0, TITLE_MAXLEN) : title;
            return title;
        }
    }
}
