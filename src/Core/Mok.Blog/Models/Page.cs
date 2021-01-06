using Mok.Blog.Enums;
using System.Collections.Generic;

namespace Mok.Blog.Models
{
    public class Page : Post, IHierarchical<Page>
    {
        public Page()
        {
            Children = new List<Page>();
        }

        /// <summary>
        /// Child pages for a parent page.
        /// </summary>
        public IList<Page> Children { get; set; }

        /// <summary>
        /// Parent page for a child.
        /// </summary>
        public Page Parent { get; set; }

        public new EPostType Type { get; } = EPostType.Page;

        /// <summary>
        /// True if the page has a parent.
        /// </summary>
        public bool IsParent => ParentId == null || ParentId == 0;

        /// <summary>
        /// True if the page has children.
        /// </summary>
        public bool HasChildren => Children.Count > 0;
    }
}
