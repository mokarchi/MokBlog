using Microsoft.AspNetCore.Http;

namespace Mok.Medias
{
    /// <summary>
    /// File system storage.
    /// </summary>
    /// user uploads file with an existing name, get a unique name
    /// the problem is olw, if user resizes an image, be aware olw sends it as new file
    /// also olw each time sends two copies of the file, orig and thumb
    public class FileSysStorageProvider : IStorageProvider
    {
        private readonly HttpRequest _request;
        
        public FileSysStorageProvider(IHttpContextAccessor httpContextAccessor)
        {
            _request = httpContextAccessor.HttpContext.Request;
        }
        /// <summary>
        /// The absolute URI endpoint to file, e.g. "https://localhost:44381" or "https://www.fanray.com".
        /// </summary>
        public string StorageEndpoint => $"{_request.Scheme}://{_request.Host}{_request.PathBase}";
    }
}
