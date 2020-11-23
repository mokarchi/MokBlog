using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Mok.Helpers;
using Mok.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ISettingService settingService;
        private readonly ILogger<CategoryService> logger;
        private readonly IDistributedCache cache;
        public CategoryService(ICategoryRepository categoryRepository,
                               ISettingService settingService,
                               IDistributedCache cache,
                               ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.settingService = settingService;
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
        /// Returns category by slug, throws <see cref="MokException"/> if category with slug is null or not found.
        /// </summary>
        public async Task<Category> GetAsync(string slug)
        {
            if (slug.IsNullOrEmpty())
                throw new MokException(EExceptionType.ResourceNotFound, "Category does not exist.");

            var cats = await GetAllAsync();
            var cat = cats.SingleOrDefault(c => c.Slug.Equals(slug, StringComparison.CurrentCultureIgnoreCase));
            if (cat == null)
            {
                throw new MokException(EExceptionType.ResourceNotFound, $"Category '{slug}' does not exist.");
            }

            return cat;
        }

        /// <summary>
        /// Creates a new <see cref="Category"/>.
        /// </summary>
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
            await cache.RemoveAsync(BlogCache.KEY_POSTS_INDEX);

            logger.LogDebug("Created {@Category}", category);
            return category;
        }

        /// <summary>
        /// Deletes a <see cref="Category"/> and reassigns posts to a default category, and 
        /// invalidates caceh for all categories.  Throws <see cref="MokException"/> if the
        /// category being deleted is the default category.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var blogSettings = await settingService.GetSettingsAsync<BlogSettings>();

            // on the UI there is no delete button on the default cat
            // therefore when there is only one category left, it'll be the default.
            if (id == blogSettings.DefaultCategoryId)
            {
                throw new MokException("Default category cannot be deleted.");
            }

            // delete
            await categoryRepository.DeleteAsync(id, 1);

            // invalidate cache
            await cache.RemoveAsync(BlogCache.KEY_ALL_CATS);
            await cache.RemoveAsync(BlogCache.KEY_POSTS_INDEX);
        }

        /// <summary>
        /// Updates an existing <see cref="Category"/>.
        /// </summary>
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
            await cache.RemoveAsync(BlogCache.KEY_POSTS_INDEX);

            // return entity
            logger.LogDebug("Updated {@Category}", entity);
            return entity;
        }

        /// <summary>
        /// Sets the id to default category.
        /// </summary>
        public async Task SetDefaultAsync(int id)
        {
            await settingService.UpsertSettingsAsync(new BlogSettings
            {
                DefaultCategoryId = id,
            });
        }

        /// <summary>
        /// Cleans category title from any html and shortens it if exceed max allow length.
        /// </summary>
        private string PrepareTitle(string title)
        {
            title = Util.CleanHtml(title);
            title = title.Length > TITLE_MAXLEN ? title.Substring(0, TITLE_MAXLEN) : title;
            return title;
        }
    }
}
