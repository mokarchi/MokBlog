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
        /// Returns a <see cref="Category"/> by id, throws FanException is category not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Admin site will call this by passing in the id of the category.
        /// </remarks>
        /// <exception cref="MokException">If category is not found.</exception>
        Task<Category> GetAsync(int id);

        /// <summary>
        /// Creates a <see cref="Category"/>, throws <see cref="MokException"/> if category title 
        /// fails validation or exists already.
        /// </summary>
        /// <returns>A category with id.</returns>
        /// <exception cref="MokException"></exception>
        Task<Category> CreateAsync(string title, string description = null);

        /// <summary>
        /// Deletes a <see cref="Category"/> by id and re-categorize its posts to the default category.
        /// </summary>
        /// <remarks>
        /// Admin console will call this by passing in an id of the category to be deleted.
        /// </remarks>
        /// <exception cref="MokException">If the id to be deleted is the default category.</exception>
        Task DeleteAsync(int id);

        Task<Category> UpdateAsync(Category category);
    }
}
