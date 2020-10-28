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
        /// Creates a <see cref="Category"/>, throws <see cref="MokException"/> if category title 
        /// fails validation or exists already.
        /// </summary>
        /// <returns>A category with id.</returns>
        /// <exception cref="MokException"></exception>
        Task<Category> CreateAsync(string title, string description = null);
    }
}
