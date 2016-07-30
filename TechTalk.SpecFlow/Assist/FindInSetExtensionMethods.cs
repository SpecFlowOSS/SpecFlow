﻿using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class FindInSetExtensionMethods
    {
        public static T FindInSet<T>(this Table table, IEnumerable<T> set)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof (T));

            var matches = set.Where(instance => InstanceMatchesTable(instance, instanceTable)).ToArray();

            if (matches.Length > 1)
                throw new ComparisonException("Multiple instances match the table");

            return matches.FirstOrDefault();
        }

        public static IEnumerable<T> FindAllInSet<T>(this Table table, IEnumerable<T> set)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof (T));

            return set.Where(instance => InstanceMatchesTable(instance, instanceTable)).ToArray();
        }

        private static bool InstanceMatchesTable<T>(T instance, Table table)
        {
            return table.Rows.All(row => !InstanceComparisonExtensionMethods.ThereIsADifference(instance, row));
        }
    }
}