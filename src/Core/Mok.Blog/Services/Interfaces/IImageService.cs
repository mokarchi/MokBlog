using System.Threading.Tasks;

namespace Mok.Blog.Services.Interfaces
{
    /// <summary>
    /// The interface for blog image service.
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Given a blog post's body html, it replaces all img tags with one that is updated for Repsonsive Images.
        /// </summary>
        /// <param name="body">A blog post's body html.</param>
        /// <returns></returns>
        Task<string> ProcessResponsiveImageAsync(string body);
    }
}
