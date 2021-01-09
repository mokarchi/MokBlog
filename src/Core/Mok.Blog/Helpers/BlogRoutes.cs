using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Mok.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Blog.Helpers
{
    public static class BlogRoutes
    {
        private const string PAGE_PARENT_RELATIVE_URL = "{0}";
        private const string PAGE_PARENT_CHILD_RELATIVE_URL = "{0}/{1}";
        private const string PAGE_EDIT_URL = "admin/compose/page/{0}";
        private const string PAGE_EDIT_NAV_URL = "admin/compose/pagenav/{0}";
        private const string PREVIEW_PARENT_RELATIVE_URL = "preview/page/{0}";
        private const string PREVIEW_PARENT_CHILD_RELATIVE_URL = "preview/page/{0}/{1}";

        private const string POST_RELATIVE_URL = "post/{0}/{1}/{2}/{3}";
        private const string PREVIEW_POST_RELATIVE_URL = "preview/post/{0}/{1}/{2}/{3}";
        private const string POST_PERMA_URL = "blog/post/{0}";
        private const string POST_EDIT_URL = "admin/compose/post/{0}";

        private const string CATEGORY_URL = "blog/{0}";
        private const string CATEGORY_RSS_URL = "blog/{0}/feed";
        private const string TAG_URL = "posts/tagged/{0}";
        private const string ARCHIVE_URL = "posts/{0}/{1}";
        /// <summary>
        /// Returns a blog category's relative link that start with "/".
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static string GetCategoryRelativeLink(string slug)
        {
            return string.Format("/" + CATEGORY_URL, slug);
        }

        /// <summary>
        /// Returns a blog category's RSS link, the returned string is relative URL that starts with "/"
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static string GetCategoryRssRelativeLink(string slug)
        {
            return string.Format("/" + CATEGORY_RSS_URL, slug);
        }

        /// <summary>
        /// Returns a blog tag's relative link that start with "/".
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static string GetTagRelativeLink(string slug)
        {
            return string.Format("/" + TAG_URL, slug);
        }

        /// <summary>
        /// Registers the blog app's routes.
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(IEndpointRouteBuilder routes)
        {
            // "blog"
            routes.MapControllerRoute("Blog", App.BLOG_APP_URL, new { controller = "Blog", action = "Index" });

            // "rsd"
            routes.MapControllerRoute("RSD", "rsd", new { controller = "Blog", action = "Rsd" });

            // "blog/post/1"
            routes.MapControllerRoute("BlogPostPerma", string.Format(POST_PERMA_URL, "{id}"),
               new { controller = "Blog", action = "PostPerma", id = 0 }, new { id = @"^\d+$" });

            // "post/2017/01/01/test-post"
            routes.MapControllerRoute("BlogPost", string.Format(POST_RELATIVE_URL, "{year}", "{month}", "{day}", "{slug}"),
                new { controller = "Blog", action = "Post", year = 0, month = 0, day = 0, slug = "" },
                new { year = @"^\d+$", month = @"^\d+$", day = @"^\d+$" });

            // "preview/post/2017/01/01/test-post"
            routes.MapControllerRoute("BlogPreview", string.Format(PREVIEW_POST_RELATIVE_URL, "{year}", "{month}", "{day}", "{slug}"),
                new { controller = "Blog", action = "PreviewPost", year = 0, month = 0, day = 0, slug = "" },
                new { year = @"^\d+$", month = @"^\d+$", day = @"^\d+$" });

            // "posts/categorized/technology"
            routes.MapControllerRoute("BlogCategory", string.Format(CATEGORY_URL, "{slug}"),
                new { controller = "Blog", action = "Category", slug = "" });

            // "posts/tagged/cs" 
            routes.MapControllerRoute("BlogTag", string.Format(TAG_URL, "{slug}"),
                new { controller = "Blog", action = "Tag", slug = "" });

            // "posts/2017/12" 
            routes.MapControllerRoute("BlogArchive", string.Format(ARCHIVE_URL, "{year}", "{month}"),
                new { controller = "Blog", action = "Archive", year = 0, month = 0 },
                new { year = @"^\d+$", month = @"^\d+$" });

            // "feed"
            routes.MapControllerRoute("BlogFeed", "feed", new { controller = "Blog", action = "Feed" });

            // "posts/categorized/technology/feed"
            routes.MapControllerRoute("BlogCategoryFeed", string.Format(CATEGORY_RSS_URL, "{slug}"),
                new { controller = "Blog", action = "CategoryFeed", slug = "" });

            // "preview/page/about"
            routes.MapControllerRoute("PagePreview", string.Format(PREVIEW_PARENT_RELATIVE_URL, "{parentSlug}"),
                new { controller = "Blog", action = "PreviewPage", parentSlug = "" });

            // "preview/page/about/ray"
            routes.MapControllerRoute("ChildPagePreview", string.Format(PREVIEW_PARENT_CHILD_RELATIVE_URL, "{parentSlug}", "{childSlug}"),
                new { controller = "Blog", action = "PreviewPage", parentSlug = "", childSlug = "" });

            // "about"
            routes.MapControllerRoute("Page", "{parentPage}",
                defaults: new { controller = "Blog", action = "Page", parentPage = "" });

            // "about/ray"
            routes.MapControllerRoute("ChildPage", "{parentPage}/{childPage}",
                defaults: new { controller = "Blog", action = "Page", parentPage = "", childPage = "" });
        }
    }
}
