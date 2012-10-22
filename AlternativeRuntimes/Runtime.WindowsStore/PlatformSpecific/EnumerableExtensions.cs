
namespace System
{
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this List<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }
    }
}
