using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            var setItems = set.ToList();

            foreach (var id in table.Header)
                if (typeof (T).GetProperties().Select(x => x.Name).Contains(id) == false)
                    throw new ComparisonException("");

            if (set.Count() != table.Rows.Count())
                throw new ComparisonException("");

            if (set.Count() == 0)
                return;

            var tableItems = table.CreateSet<T>().ToList();

            for (var index = 0; index < tableItems.Count(); index++)
            {
                foreach (var id in table.Header)
                {
                    var propertyValue = setItems[index].GetPropertyValue(id);
                    var tableValue = tableItems[index].GetPropertyValue(id);

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
}