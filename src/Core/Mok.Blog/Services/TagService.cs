using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository tagRepository;
        private readonly IDistributedCache cache;
        private readonly ILogger<TagService> logger;

        public TagService(ITagRepository tagRepository,
                          IDistributedCache cache,
                          ILogger<TagService> logger)
        {
            this.tagRepository = tagRepository;
            this.cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// Returns all tags, cached after calls to DAL.
        /// </summary>
        public async Task<List<Tag>> GetAllAsync()
        {
            return await cache.GetAsync(BlogCache.KEY_ALL_TAGS, BlogCache.Time_AllTags, async () => {
                return await tagRepository.GetListAsync();
            });
        }
    }
}
