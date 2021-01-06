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
    }
}
