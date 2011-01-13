using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> AsSingleItemEnumerable<T>(this T item) where T : class
        {
            if (item == null)
                yield break;

            yield return item;
        }

        public static IEnumerable<T> AppendIfNotNull<T>(this IEnumerable<T> items, T item) where T : class
        {
            if (items == null) throw new ArgumentNullException("items");

            if (item == null)
                return items;

            return items.Append(item);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T item) where T : class
        {
            if (items == null) throw new ArgumentNullException("items");
            if (item == null) throw new ArgumentNullException("item");

            return items.Concat(item.AsSingleItemEnumerable());
        }

        public static IEnumerable<TItem> TakeUntilItemExclusive<TItem>(this IEnumerable<TItem> list, TItem item)
        {
            return list.TakeWhile(it => !it.Equals(item));
        }

        public static IEnumerable<TItem> TakeUntilItemInclusive<TItem>(this IEnumerable<TItem> list, TItem item) where TItem : class
        {
            return list.TakeWhile(it => !it.Equals(item)).Append(item);
        }

        public static IEnumerable<TItem> SkipFromItemInclusive<TItem>(this IEnumerable<TItem> list, TItem item)
        {
            return list.SkipWhile(it => !it.Equals(item));
        }

        public static IEnumerable<TItem> SkipFromItemExclusive<TItem>(this IEnumerable<TItem> list, TItem item)
        {
            return list.SkipWhile(it => !it.Equals(item)).Skip(1);
        }
    }
}