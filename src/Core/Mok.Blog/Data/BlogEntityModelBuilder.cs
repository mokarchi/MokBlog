using Microsoft.EntityFrameworkCore;
using Mok.Blog.Models;
using MokCore.Data;

namespace Mok.Blog.Data
{
    public class BlogEntityModelBuilder : IEntityModelBuilder
    {
        public void CreateModel(ModelBuilder builder)
        {
            builder.Entity<Post>(entity =>
            {
                entity.ToTable("Blog_Post");
                entity.HasIndex(e => e.Slug);
                entity.HasIndex(e => new { e.Type, e.Status, e.CreatedOn, e.Id });
                entity.HasIndex(e => e.ParentId);
                entity.HasIndex(e => e.UserId);
            });

            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Blog_Category");
                entity.HasKey(e => e.Id).IsClustered(clustered: false);
                entity.HasIndex(e => e.Slug).IsUnique().IsClustered();
            });

            builder.Entity<Tag>(entity =>
            {
                entity.ToTable("Blog_Tag");
                entity.HasKey(e => e.Id).IsClustered(clustered: false);
                entity.HasIndex(e => e.Slug).IsUnique().IsClustered();
            });

            builder.Entity<PostTag>(entity =>
            {
                entity.ToTable("Blog_PostTag");
                entity.HasKey(e => new { e.PostId, e.TagId });
            });
        }
    }
}
