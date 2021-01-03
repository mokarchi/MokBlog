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
        public const string KEY_POSTS_RECENT = "BlogPostsRecent";
        public const string KEY_ALL_ARCHIVES = "BlogArchives";
        public const string KEY_POST_COUNT = "BlogPostCount";
        public const string KEY_POST = "BlogPost_{0}_{1}_{2}_{3}";
        public const string KEY_PAGE = "Page_{0}";

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

        /// <summary>
        /// 2 minutes.
        /// </summary>
        public static readonly TimeSpan Time_ChildPage = new TimeSpan(0, 2, 0);

        /// <summary>
        /// 10 minutes.
        /// </summary>
        public static readonly TimeSpan Time_ParentPage = new TimeSpan(0, 10, 0);
    }
}
