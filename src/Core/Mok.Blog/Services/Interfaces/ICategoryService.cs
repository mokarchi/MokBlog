using Mok.Blog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mok.Exceptions;

namespace Mok.Blog.Services.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Returns all categories.
        /// </summary>
        Task<List<Category>> GetAllAsync();

        /// <summary>
        /// Returns a <see cref="Category"/> by id, throws <see cref="MokException"/> is category not found.
        /// <summary>
        Task<Category> GetAsync(int id);

        /// <summary>
        /// Returns a <see cref="Category"/> by slug, throws <see cref="MokException"/> is category not found.
        /// </summary>
        Task<Category> GetAsync(string slug);

        /// <summary>
        /// Creates a <see cref="Category"/>, throws <see cref="MokException"/> if category title 
        /// fails validation or exists already.
        /// </summary>
        Task<Category> CreateAsync(string title, string description = null);

        /// <summary>
        /// Deletes a <see cref="Category"/> by id and re-categorize its posts to the default category.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Updates a <see cref="Category"/>, throws <see cref="MokException"/> if category title 
        /// or slug fails validation or exists already.
        /// </summary>
        Task<Category> UpdateAsync(Category category);

        /// <summary>
        /// Sets the id to default category.
        /// </summary>
        Task SetDefaultAsync(int id);
    }
}
