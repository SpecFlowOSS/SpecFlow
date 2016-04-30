using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class FindInSetExtensionMethods
    {
        public static T FindInSet<T>(this Table table, IEnumerable<T> set)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof (T));

            foreach (var instance in set.Where(instance => InstanceMatchesTable(instance, instanceTable)))
                return instance;

            throw new ComparisonException("No instance in the set matches the table");
        }

        private static bool InstanceMatchesTable<T>(T instance, Table table)
        {
            return table.Rows.All(row => !InstanceComparisonExtensionMethods.ThereIsADifference(instance, row));
        }
    }
}