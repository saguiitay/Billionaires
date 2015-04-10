using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Billionaires.Helpers;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Billionaires.Cache
{
    /// <summary>
    /// Save object to local storage, serializes as json and writes object to a file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StorageHelper<T>
    {
        private const string FileExtension = ".json";

        private readonly ApplicationData _appData = ApplicationData.Current;
        private readonly StorageType _storageType;
        private readonly string _subFolder;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="storageType"></param>
        /// <param name="subFolder"></param>
        public StorageHelper(StorageType storageType, string subFolder = null)
        {
            _storageType = storageType;
            _subFolder = subFolder;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName"></param>
        public async Task DeleteAsync(string fileName)
        {
            fileName = fileName + FileExtension;
            StorageFolder folder = await GetFolderAsync().ConfigureAwait(false);

            if (await folder.ContainsFileAsync(fileName).ConfigureAwait(false))
            {
                var file = await folder.GetFileAsync(fileName);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        /// <summary>
        /// Save object from file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public async Task SaveAsync(T obj, string fileName)
        {
            if (obj == null)
                return;

            fileName = fileName + FileExtension;
            //Get file
            StorageFolder folder = await GetFolderAsync().ConfigureAwait(false);
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            //Serialize object
            var json = JsonConvert.SerializeObject(obj);

            using (var stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Load object from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<T> LoadAsync(string fileName)
        {
            fileName = fileName + FileExtension;
            StorageFolder folder = await GetFolderAsync().ConfigureAwait(false);

            if (await folder.ContainsFileAsync(fileName).ConfigureAwait(false))
            {
                StorageFile file = await folder.GetFileAsync(fileName);

                string data;
                IRandomAccessStream accessStream = await file.OpenReadAsync();
                if (accessStream.Size == 0)
                    return default(T);

                using (Stream stream = accessStream.AsStreamForRead((int) accessStream.Size))
                {
                    var content = new byte[stream.Length];
                    await stream.ReadAsync(content, 0, (int)stream.Length).ConfigureAwait(false);
                    data = Encoding.UTF8.GetString(content, 0, content.Length);
                }

                //Deserialize to object
                var result = JsonConvert.DeserializeObject<T>(data);

                return result;
            }
            return default(T);
        }

        /// <summary>
        /// Get folder based on storagetype
        /// </summary>
        /// <returns></returns>
        public async Task<StorageFolder> GetFolderAsync()
        {
            StorageFolder folder;
            switch (_storageType)
            {
                case StorageType.Roaming:
                    folder = _appData.RoamingFolder;
                    break;
                case StorageType.Local:
                    folder = _appData.LocalFolder;
                    break;
                case StorageType.Temporary:
                    folder = _appData.TemporaryFolder;
                    break;
                default:
                    throw new Exception(String.Format("Unknown StorageType: {0}", _storageType));
            }

            if (!string.IsNullOrEmpty(_subFolder))
            {
                folder = await folder.CreateFolderAsync(_subFolder, CreationCollisionOption.OpenIfExists);
            }

            return folder;
        }

        /// <summary>
        /// Clear the complete cache
        /// </summary>
        /// <returns></returns>
        public static Task ClearLocalAll()
        {
            return Task.Run(async () =>
                {
                    var storage = new StorageHelper<object>(StorageType.Local);
                    var folder = await storage.GetFolderAsync().ConfigureAwait(false);

                    foreach (var sub in await folder.GetFoldersAsync())
                    {
                        try
                        {
                            await sub.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }

                    foreach (var file in await folder.GetFilesAsync())
                    {
                        try
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                });
        }

    }
}