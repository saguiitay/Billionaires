using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Billionaires.Helpers
{
    public static class StorageFolderExtensions
    {

        /// <summary>
        /// Extension method to check if file exist in folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> ContainsFileAsync(this StorageFolder folder, string fileName)
        {
            //This looks nicer, but gave a COM errors in some situations
            //TODO: Check again in final release of Windows 8 (or 9, or 10)
            //return (await folder.GetFilesAsync()).Where(file => file.Name == fileName).Any();

            try
            {
                return (await folder.GetFilesAsync()).Any(f => f.Name == fileName);
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}