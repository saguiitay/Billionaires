using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billionaires.Helpers
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Similar in nature to Parallel.ForEach, with the primary difference being that Parallel.ForEach is a synchronous method and uses synchronous delegates.
        /// http://blogs.msdn.com/b/pfxteam/archive/2012/03/05/10278165.aspx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="dop"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from item in source
                select Task.Run(async delegate
                    {
                        await body(item).ConfigureAwait(false);
                    }));
        }
    }
}