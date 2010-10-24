using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            var id = table.Header.First();
            if (set.First().GetPropertyValue(id) != tableItems.First().GetPropertyValue(id))
                throw new ComparisonException("");

            if (table.Header.Count() > 1)
                id = table.Header.ToList()[1];
                if (set.First().GetPropertyValue(id).ToString() != tableItems.First().GetPropertyValue(id).ToString())
                    throw new ComparisonException("");
        }
    }
}
