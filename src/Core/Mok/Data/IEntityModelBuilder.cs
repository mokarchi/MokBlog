using Microsoft.EntityFrameworkCore;

namespace MokCore.Data
{
    /// <summary>
    /// The model builder every project that persists data to a database needs to implement.
    /// </summary>
    public interface IEntityModelBuilder
    {
        /// <summary>
        /// Creates the entity model for the project.
        /// </summary>
        /// <param name="builder"></param>
        void CreateModel(ModelBuilder builder);
    }
}
