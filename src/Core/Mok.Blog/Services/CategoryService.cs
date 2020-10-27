﻿using Microsoft.Extensions.Logging;
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
        public CategoryService(ICategoryRepository categoryRepository,
                               ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
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

        public async Task<List<Category>> GetAllAsync()
        {
            return await categoryRepository.GetListAsync();
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

            logger.LogDebug("Created {@Category}", category);
            return category;
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
