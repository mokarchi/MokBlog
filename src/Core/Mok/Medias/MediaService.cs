using System.Threading.Tasks;

namespace Mok.Medias
{
    /// <summary>
    /// The media service manages media files: resizes image file, generates unique filename, passes file to storage, 
    /// generates handler url.  
    /// </summary>
    /// <remarks>
    /// See Media model class and admin Media.cshtml page for more information.
    /// </remarks>
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepo;

        public MediaService(IMediaRepository mediaRepo)
        {
            _mediaRepo = mediaRepo;
        }

        /// <summary>
        /// Returns <see cref="Media"/> by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Media> GetMediaAsync(int id) => await _mediaRepo.GetAsync(id);

        /// <summary>
        /// Returns <see cref="Media"/> by filename and upload datetime.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<Media> GetMediaAsync(string fileName, int year, int month) =>
            await _mediaRepo.GetAsync(fileName, year, month);
    }
}
