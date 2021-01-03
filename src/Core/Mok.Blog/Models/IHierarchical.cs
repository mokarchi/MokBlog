using System.Collections.Generic;

namespace Mok.Blog.Models
{
    public interface IHierarchical<T> where T : IHierarchical<T>
    {
        T Parent { get; set; }

        IList<T> Children { get; set; }
    }
}
