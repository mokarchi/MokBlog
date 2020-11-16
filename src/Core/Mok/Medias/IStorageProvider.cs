namespace Mok.Medias
{
    /// <summary>
    /// The storage provider save the incoming file whether its byte[] or stream into the storage.
    /// It makes sure it gets a unique filename by looking at what is already in storage.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// The absolute URI endpoint to resource.
        /// </summary>
        string StorageEndpoint { get; }
    }
}
