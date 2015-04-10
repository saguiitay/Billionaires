// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq;
using Microsoft.Phone.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Billionaires.Helpers
{
    public class AlphaKeyGroup<T> : List<T>
    {
        const string GlobeGroupKey = "\uD83C\uDF10";

        /// <summary>
        /// The Key of this group.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Public ctor.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        public AlphaKeyGroup(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Create a list of AlphaGroup{T} with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        private static List<AlphaKeyGroup<T>> CreateDefaultGroups(SortedLocaleGrouping slg)
        {
            return slg.GroupDisplayNames
                .Select(key => key == "..." ? GlobeGroupKey : key)
                .Select(key => new AlphaKeyGroup<T>(key))
                .ToList();
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping 
        /// using the current threads culture to determine which alpha keys to
        /// include.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="keySelector"></param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, Func<T, string> keySelector, bool sort)
        {
            return CreateGroups(
                items,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                keySelector,
                sort);
        }

        public static List<AlphaKeyGroup<T>> CreateGroupsByKey(IEnumerable<T> items, Func<T, string> keySelector, Func<T, string> sortSelector)
        {
            return CreateGroupsByKey(
                items,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                keySelector,
                true, 
                sortSelector);
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="keySelector"></param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, Func<T, string> keySelector, bool sort)
        {
            var slg = new SortedLocaleGrouping(ci);
            List<AlphaKeyGroup<T>> list = CreateDefaultGroups(slg);

            foreach (T item in items)
            {
                int index;
                {
                    index = slg.GetGroupIndex(keySelector(item));
                }

                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }

            if (sort)
            {
                foreach (AlphaKeyGroup<T> group in list)
                {
                    group.Sort((c0, c1) => ci.CompareInfo.Compare(keySelector(c0), keySelector(c1)));
                }
            }

            return list;
        }

        public static List<AlphaKeyGroup<T>> CreateGroupsByKey(IEnumerable<T> items, CultureInfo ci,
                                                               Func<T, string> keySelector, bool sort,
                                                               IComparer<T> sortSelector)
        {
            var list = items
                .Select(keySelector)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new AlphaKeyGroup<T>(x))
                .ToList();

            foreach (T item in items)
            {
                int index = list.FindIndex(x => x.Key == keySelector(item));

                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }

            if (sort)
            {
                if (sortSelector != null)
                {
                    foreach (AlphaKeyGroup<T> group in list)
                    {
                        group.Sort(sortSelector);
                    }
                }
                else
                {
                    foreach (AlphaKeyGroup<T> group in list)
                    {
                        group.Sort((c0, c1) => ci.CompareInfo.Compare(keySelector(c0), keySelector(c1)));
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroupsByKey(IEnumerable<T> items, CultureInfo ci, Func<T, string> keySelector, bool sort, Func<T, string> sortSelector)
        {
            var list = items
                .Select(keySelector)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new AlphaKeyGroup<T>(x))
                .ToList();

            foreach (T item in items)
            {
                int index = list.FindIndex(x => x.Key == keySelector(item));

                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }

            if (sort)
            {
                if (sortSelector != null)
                {
                    foreach (AlphaKeyGroup<T> group in list)
                    {
                        group.Sort((c0, c1) => ci.CompareInfo.Compare(sortSelector(c0), sortSelector(c1)));
                    }
                }
                else
                {
                    foreach (AlphaKeyGroup<T> group in list)
                    {
                        group.Sort((c0, c1) => ci.CompareInfo.Compare(keySelector(c0), keySelector(c1)));
                    }
                }
            }

            return list;
        }
    }
}
