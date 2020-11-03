using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mok.Blog.Data;
using Mok.Blog.Models;
using Mok.Blog.Services;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Mok.Blog.Tests.Services
{
    public class CategoryServiceTest
    {
        private readonly ICategoryService categoryService;
        private readonly Mock<ICategoryRepository> catRepoMock = new Mock<ICategoryRepository>();
        private readonly IDistributedCache cache;

        public CategoryServiceTest()
        {
            // logger
            var serviceProvider = new ServiceCollection().AddMemoryCache().AddLogging().BuildServiceProvider();
            cache = new MemoryDistributedCache(serviceProvider.GetService<IOptions<MemoryDistributedCacheOptions>>());
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<CategoryService>();

            // setup the default category in db
            var defaultCat = new Category { Id = 1, Title = "Web Development", Slug = "web-development" };
            catRepoMock.Setup(c => c.GetAsync(1)).Returns(Task.FromResult(defaultCat));
            catRepoMock.Setup(r => r.GetListAsync()).Returns(Task.FromResult(new List<Category> { defaultCat }));

            // cat service
            categoryService = new CategoryService(catRepoMock.Object, cache, logger);
        }

        /// <summary>
        /// Creates a category with empty title will throw MokException.
        /// </summary>
        /// <param name="title"></param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void Create_category_with_empty_title_throws_MokException(string title)
        {
            await Assert.ThrowsAsync<MokException>(() => categoryService.CreateAsync(title));
        }

        /// <summary>
        /// Creates a category with a title that already exists throws MokException.
        /// </summary>
        [Fact]
        public async void Create_category_throws_MokException_if_title_already_exists()
        {
            var title = "web development";
            var ex = await Assert.ThrowsAsync<MokException>(() => categoryService.CreateAsync(title));

            Assert.Equal("'web development' already exists.", ex.Message);
        }
    }
}
