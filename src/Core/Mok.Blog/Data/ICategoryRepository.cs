﻿using Mok.Blog.Models;
using Mok.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mok.Blog.Data
{
    public interface ICategoryRepository : IRepository<Category>
    {
        /// <summary>
        /// Returns a list of <see cref="Category"/>, the returned objects are not tracked.
        /// The returned list are order by alphabetically on <see cref="Category.Title"/>.
        /// </summary>
        Task<List<Category>> GetListAsync();

        /// <summary>
        /// Deletes a <see cref="Category"/> by id and re-categorize its posts to the given 
        /// default category id.
        /// </summary>
        Task DeleteAsync(int id, int defaultCategoryId);
    }
}
