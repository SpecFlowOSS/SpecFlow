using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            foreach (var id in table.Header)
                if (typeof (T).GetProperties().Select(x => x.Name).Contains(id) == false)
                    throw new ComparisonException("");

            if (set.Count() != table.Rows.Count())
                throw new ComparisonException("");

            if (set.Count() == 0)
                return;

            var tableItems = table.CreateSet<T>();

            foreach (var id in table.Header)
            {
                var propertyValue = set.First().GetPropertyValue(id);
                var tableValue = tableItems.First().GetPropertyValue(id);

                if (propertyValue != null && tableValue == null)
                    throw new ComparisonException("");

                if (propertyValue == null && tableValue != null)
                    throw new ComparisonException("");

                if (propertyValue != null && tableValue != null)
                    if (propertyValue.ToString() != tableValue.ToString())
                        throw new ComparisonException("");
            }
        }
    }
}