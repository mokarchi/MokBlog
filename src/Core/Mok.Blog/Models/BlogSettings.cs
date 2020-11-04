using Mok.Settings;

namespace Mok.Blog.Models
{
    /// <summary>
    /// Settings for the blog application.
    /// </summary>
    /// <remarks>
    /// This is saved in the meta table, it's ok to add or remove properties here.
    /// </remarks>
    public class BlogSettings : ISettings
    {
        /// <summary>
        /// There must be one default category. Default 1.
        /// </summary>
        public int DefaultCategoryId { get; set; } = 1;
    }
}
