using System.Threading.Tasks;

namespace Mok.Medias
{
    /// <summary>
    /// The media service manages uploaded media files.
    /// </summary>
    /// <remarks>
    /// For images it resizes them, stores them away with storage provider, gets a unique filename,
    /// encode a title from filename, generates an image url.
    /// </remarks>
    public interface IMediaService
    {
        /// <summary>
        /// Returns <see cref="Media"/> by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Media> GetMediaAsync(int id);

        /// <summary>
        /// Returns <see cref="Media"/> by filename and upload datetime.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<Media> GetMediaAsync(string fileName, int year, int month);
    }
}
