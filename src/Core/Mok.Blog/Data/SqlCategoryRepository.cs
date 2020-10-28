using Microsoft.EntityFrameworkCore;
using Mok.Blog.Models;
using Mok.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mok.Blog.Data
{
    /// <summary>
    /// Sql implementation of the <see cref="ICategoryRepository"/> contract.
    /// </summary>
    /// <remarks>
    /// Category specific data access methods.
    /// </remarks>
    public class SqlCategoryRepository : EntityRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public SqlCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        /// <summary>
        /// Returns a list of <see cref="Category"/>, the returned objects are not tracked.
        /// The returned list are order by alphabetically on <see cref="Category.Title"/>.
        /// </summary>
        public async Task<List<Category>> GetListAsync()
        {
            return await _entities.Select(
                c => new Category
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug,
                    Description = c.Description
                }).OrderBy(c => c.Title).ToListAsync();
        }
    }
}
