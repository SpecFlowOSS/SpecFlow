using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Compatibility
{
    public static class ListHelper
    {
        public static void RemoveAll<T>(this List<T> list, Predicate<T> predicate)
        {
            for(int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                    list.RemoveAt(i);
            }
        }
    }
}
