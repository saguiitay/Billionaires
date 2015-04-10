using System;

namespace Billionaires.Helpers
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Converts Uri to cache key extension method
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string ToCacheKey(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            string result = uri.AbsolutePath
                               .Replace(".", "_")
                               .Replace("/", "_")
                               .Replace("?", "_")
                               .Replace("&", "_")
                               .Replace(":", "_");

            return result;
        }

    }
}