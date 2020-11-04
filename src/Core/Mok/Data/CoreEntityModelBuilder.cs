using Microsoft.EntityFrameworkCore;
using MokCore.Data;

namespace Mok.Data
{
    /// <summary>
    /// The core entity model.
    /// </summary>
    public class CoreEntityModelBuilder : IEntityModelBuilder
    {
        public void CreateModel(ModelBuilder builder)
        {
            builder.Entity<Meta>(entity =>
            {
                entity.ToTable("Core_Meta");
                entity.HasKey(e => e.Id).IsClustered(clustered: false);
                entity.HasIndex(e => new { e.Type, e.Key }).IsUnique().IsClustered();
            });
        }
    }
}
