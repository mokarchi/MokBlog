using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Services;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Moq;
using Xunit;

namespace Mok.Blog.Tests.Services
{
    public class CategoryServiceTest
    {
        private readonly ICategoryService categoryService;
        private readonly Mock<ICategoryRepository> catRepoMock = new Mock<ICategoryRepository>();

        public CategoryServiceTest()
        {
            // logger
            var serviceProvider = new ServiceCollection().AddMemoryCache().AddLogging().BuildServiceProvider();
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<CategoryService>();

            // cat service
            categoryService = new CategoryService(catRepoMock.Object, logger);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void Create_category_with_empty_title_throws_MokException(string title)
        {
            await Assert.ThrowsAsync<MokException>(() => categoryService.CreateAsync(title));
        }
    }
}
