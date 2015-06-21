using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public static class FindInSetExtensionMethods
    {
        public static T FindInSet<T>(this Table table, IEnumerable<T> set)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof(T));

            foreach (var instance in set)
            {
                if (InstanceMatchesTable(instance, instanceTable))
                {
                    return instance;
                }
            }

            throw new ComparisonException("No instance in the set matches the table");
        }

        private static bool InstanceMatchesTable<T>(T instance, Table table)
        {
            foreach (var row in table.Rows)
            {
                if (InstanceHelper.ThereIsADifference(instance, row))
                {
                    return false;
                }
            }
            return true;
        }
    }
}