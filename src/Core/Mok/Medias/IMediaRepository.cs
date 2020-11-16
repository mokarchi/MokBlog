using Mok.Data;
using System.Threading.Tasks;

namespace Mok.Medias
{
    /// <summary>
    /// Contract for a media repository.
    /// </summary>
    /// <remarks>
    /// This class should not be used outside of this DLL except when register for DI in Startup.cs, 
    /// all media operations should be called through <see cref="IMediaService"/>.
    /// </remarks>
    public interface IMediaRepository : IRepository<Media>
    {
        /// <summary>
        /// Returns <see cref="Media"/> by filename and upload date, returns null if not found.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<Media> GetAsync(string fileName, int year, int month);
    }
}
