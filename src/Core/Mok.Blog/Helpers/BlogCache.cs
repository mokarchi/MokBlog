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

        /// <summary>
        /// 10 minutes.
        /// </summary>
        public static readonly TimeSpan Time_AllCats = new TimeSpan(0, 10, 0);
    }
}
