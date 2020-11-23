using Mok.Blog.Enums;
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

        /// <summary>
        /// Number of blog posts to show on a page. Default 10.
        /// </summary>
        public int PostPerPage { get; set; } = 10;

        /// <summary>
        /// What type of display for each blog post in a list of posts.
        /// </summary>
        public EPostListDisplay PostListDisplay { get; set; } = EPostListDisplay.FullBody;

        /// <summary>
        /// Whether to allow people to post comments on blog posts.
        /// </summary>
        /// <remarks>
        /// When turning on this setting, it will not hide any existing comments, it will not allow
        /// visitors to post comments on new or existing posts.
        /// </remarks>
        public bool AllowComments { get; set; } = true;

        /// <summary>
        /// Which comment system to use. Default Disqus.
        /// </summary>
        public ECommentProvider CommentProvider { get; set; } = ECommentProvider.Disqus;

        /// <summary>
        /// Disqus shortname. Default is null.
        /// </summary>
        public string DisqusShortname { get; set; }

        /// <summary>
        /// Gets or sets whether feed should show full text or excerpt. Default false.
        /// </summary>
        /// <remarks>
        /// For each article in a feed, show
        /// </remarks>
        public bool FeedShowExcerpt { get; set; } = false;
    }
}
