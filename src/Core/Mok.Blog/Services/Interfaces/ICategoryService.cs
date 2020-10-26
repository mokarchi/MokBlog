using Mok.Blog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mok.Blog.Services.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Returns all categories.
        /// </summary>
        Task<List<Category>> GetAllAsync();
    }
}
