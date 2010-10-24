using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class ComparisonHelper
    {
        public static void CompareToInstance<T>(this Table table, T instance)
        {
            var differences = FindAnyDifferences(table, instance);

            if (ThereAreAnyDifferences(differences))
            {
                throw new ComparisonException(
                    differences.Aggregate(@"The following fields did not match:",
                                          (sum, next) => sum + ("\r\n" +
                                                                string.Format("{0}: Expected <{1}>, Actual <{2}>",
                                                                              next.Property, next.Expected,
                                                                              next.Actual))));
            }
        }

        private static IEnumerable<Difference> FindAnyDifferences<T>(Table table, T instance)
        {
            return from row in table.Rows
                   where TheValuesDoNotMatch(instance, row)
                   select CreateDifferenceForThisRow(instance, row);
        }

        private static bool ThereAreAnyDifferences(IEnumerable<Difference> differences)
        {
            return differences.Count() > 0;
        }

        private static bool TheValuesDoNotMatch<T>(T instance, TableRow row)
        {
            return instance.GetPropertyValue(row["Field"]) != row["Value"];
        }

        private static Difference CreateDifferenceForThisRow<T>(T instance, TableRow row)
        {
            return new Difference
                       {
                           Property = row["Field"],
                           Expected = row["Value"],
                           Actual = instance.GetPropertyValue(row["Field"])
                       };
        }

        private class Difference
        {
            public string Property { get; set; }
            public object Expected { get; set; }
            public object Actual { get; set; }
        }
    }

    public class ComparisonException : Exception
    {
        public ComparisonException(string message)
            : base(message)
        {
        }
    }
}