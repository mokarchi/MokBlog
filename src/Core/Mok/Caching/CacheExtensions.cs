using Mok.Exceptions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Returns T from cache if available, else acquire T and puts it into the cache before returning it.
        /// </summary>
        public async static Task<T> GetAsync<T>(this IDistributedCache cache, string key,
           TimeSpan cacheTime, Func<Task<T>> acquire, bool includeTypeName = false) where T : class
        {
            try
            {
                var str = await cache.GetStringAsync(key);

                if (str != null)
                {
                    return includeTypeName ?
                        JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }) :
                        JsonConvert.DeserializeObject<T>(str);
                }
                else
                {
                    var task = acquire();
                    if (task != null && task.Result != null)
                    {
                        str = includeTypeName ?
                            await Task.Factory.StartNew(() => JsonConvert.SerializeObject(task.Result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })) :
                            await Task.Factory.StartNew(() => JsonConvert.SerializeObject(task.Result));

                        await cache.SetStringAsync(key, str, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheTime });
                    }

                    return await task;
                }
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    if (e is MokException) throw e;
                }
                throw new MokException(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
    }
}
