using Mok.Blog.Models;
using Mok.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mok.Blog.Data
{
    public interface ITagRepository : IRepository<Tag>
    {
        /// <summary>
        /// Returns all tags or empty list if no tags found. The returned list is ordered by 
        /// <see cref="Tag.Count"/> desc.
        /// </summary>
        Task<List<Tag>> GetListAsync();
    }
}
