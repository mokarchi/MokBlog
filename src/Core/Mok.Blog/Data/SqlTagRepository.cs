using Mok.Blog.Models;
using Mok.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mok.Blog.Data
{
    /// <summary>
    /// Sql implementation of the <see cref="ITagRepository"/> contract.
    /// </summary>
    public class SqlTagRepository : EntityRepository<Tag>, ITagRepository
    {
        private readonly ApplicationDbContext _db;
        public SqlTagRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all tags or empty list if no tags found. The returned list is ordered by 
        /// <see cref="Tag.Count"/> desc and then by <see cref="Tag.Title"/>.
        /// </summary>
        public async Task<List<Tag>> GetListAsync()
        {
            return await (from t in _entities
                          select new Tag
                          {
                              Id = t.Id,
                              Title = t.Title,
                              Slug = t.Slug,
                              Description = t.Description
                          }).ToListAsync();
        }
    }
}
