using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Blog.Helpers
{
    /// <summary>
    /// Blog cache keys and intervals.
    /// </summary>
    public class BlogCache
    {
        public const string KEY_ALL_CATS = "BlogCategories";
        public const string KEY_ALL_TAGS = "BlogTags";
        public const string KEY_POSTS_INDEX = "BlogPostsIndex";

        /// <summary>
        /// 10 minutes.
        /// </summary>
        public static readonly TimeSpan Time_AllCats = new TimeSpan(0, 10, 0);

        /// <summary>
        /// 10 minutes.
        /// </summary>
        public static readonly TimeSpan Time_AllTags = new TimeSpan(0, 10, 0);

        /// <summary>
        /// 10 minutes.
        /// </summary>
        public static readonly TimeSpan Time_Posts_Index = new TimeSpan(0, 10, 0);
    }
}
