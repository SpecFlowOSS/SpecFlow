using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            if (set.Count() != table.Rows.Count())
                throw new ComparisonException("");

            if (set.Count() == 0)
                return;

            var tableItems = table.CreateSet<T>();

            foreach (var id in table.Header)
            {
                if (set.First().GetPropertyValue(id).ToString() != tableItems.First().GetPropertyValue(id).ToString())
                    throw new ComparisonException("");
            }
        }
    }
}