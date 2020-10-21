using Microsoft.EntityFrameworkCore;
using Mok.Blog.Models;
using MokCore.Data;

namespace Mok.Blog.Data
{
    public class BlogEntityModelBuilder : IEntityModelBuilder
    {
        public void CreateModel(ModelBuilder builder)
        {
            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Blog_Category");
                entity.HasKey(e => e.Id).IsClustered(clustered: false);
                entity.HasIndex(e => e.Slug).IsUnique().IsClustered();
            });
        }
    }
}
