using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ILogger<CategoryService> logger;
        public CategoryService(ICategoryRepository categoryRepository,
                               ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.logger = logger;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await categoryRepository.GetListAsync();
        }
    }
}
