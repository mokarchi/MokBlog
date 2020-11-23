using System.Threading.Tasks;

namespace Mok.Settings
{
    /// <summary>
    /// The settings service contract.
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Returns a type of <see cref="ISettings"/>.
        /// </summary>
        Task<T> GetSettingsAsync<T>() where T : class, ISettings, new();

        /// <summary>
        /// Creates or updates a Settings, if a particular setting exists then updates it, else inserts it.
        /// </summary>
        Task<T> UpsertSettingsAsync<T>(T settings) where T : class, ISettings, new();
    }
}
