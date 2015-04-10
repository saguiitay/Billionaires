using System;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;
using Windows.Storage;

namespace Billionaires.Cache
{
    /// <summary>
    /// Stores objects as json in the localstorage
    /// </summary>
    public static class JsonCache
    {
        private const string CacheFolder = "_jsoncache";

        /// <summary>
        /// Get object based on key, or generate the value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="generate"></param>
        /// <param name="expireDate"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public async static Task<T> GetAsync<T>(string key, Func<Task<T>> generate, DateTime? expireDate = null, bool forceRefresh = false)
        {
            object value;

            //Force bypass of cache?
            if (!forceRefresh)
            {
                //Check cache
                value = await GetFromCache<T>(key).ConfigureAwait(false);
                if (value != null)
                {
                    return (T)value;
                }
            }

            value = await generate().ConfigureAwait(false);
            if (value == null)
                return default(T);

            try
            {
                await Set(key, value, expireDate).ConfigureAwait(false);
            }
            catch
            {}

            return (T)value;

        }

        /// <summary>
        /// Get value from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private async static Task<T> GetFromCache<T>(string key)
        {
            var storage = new StorageHelper<CacheObject<T>>(StorageType.Local, CacheFolder);

            //Get cache value
            var value = await storage.LoadAsync(key).ConfigureAwait(false);

            if (value == null)
                return default(T);
            if (value.IsValid)
                return value.File;
            
            //Delete old value
            await Delete(key).ConfigureAwait(false);

            return default(T);
        }

        /// <summary>
        /// Set value in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireDate"></param>
        /// <returns></returns>
        private static Task Set<T>(string key, T value, DateTime? expireDate = null)
        {
            var storage = new StorageHelper<CacheObject<T>>(StorageType.Local, CacheFolder);

            var cacheFile = new CacheObject<T> { File = value, ExpireDateTime = expireDate };

            return storage.SaveAsync(cacheFile, key);
        }

        /// <summary>
        /// Delete key from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static Task Delete(string key)
        {
            var storage = new StorageHelper<object>(StorageType.Local, CacheFolder);
            return storage.DeleteAsync(key);
        }

        /// <summary>
        /// Clear the complete cache
        /// </summary>
        /// <returns></returns>
        public static Task ClearAll()
        {
            return Task.Run(async () =>
                {
                    var storage = new StorageHelper<object>(StorageType.Local, CacheFolder);
                    var folder = await storage.GetFolderAsync().ConfigureAwait(false);

                    try
                    {
                        await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                });
        }
    }
}