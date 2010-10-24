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
                ThrowAnExceptionThatDescribesThoseDifferences(differences);
        }

        private static void ThrowAnExceptionThatDescribesThoseDifferences(IEnumerable<Difference> differences)
        {
            throw new ComparisonException(CreateDescriptiveErrorMessage(differences));
        }

        private static string CreateDescriptiveErrorMessage(IEnumerable<Difference> differences)
        {
            return differences.Aggregate(@"The following fields did not match:",
                                         (sum, next) => sum + ("\r\n" + DescribeTheErrorForThisDifference(next)));
        }

        private static string DescribeTheErrorForThisDifference(Difference difference)
        {
            if (difference.DoesNotExist)
                return string.Format("{0}: Property does not exist", difference.Property);

            return string.Format("{0}: Expected <{1}>, Actual <{2}>",
                                 difference.Property, difference.Expected,
                                 difference.Actual);
        }

        private static IEnumerable<Difference> FindAnyDifferences<T>(Table table, T instance)
        {
            return from row in table.Rows
                   where ThePropertyDoesNotExist(instance, row) || TheValuesDoNotMatch(instance, row)
                   select CreateDifferenceForThisRow(instance, row);
        }

        private static bool ThereAreAnyDifferences(IEnumerable<Difference> differences)
        {
            return differences.Count() > 0;
        }

        private static bool ThePropertyDoesNotExist<T>(T instance, TableRow row)
        {
            return instance.GetType().GetProperties()
                .Any(property => property.Name == row["Field"]) == false;
        }

        private static bool TheValuesDoNotMatch<T>(T instance, TableRow row)
        {
            return instance.GetPropertyValue(row["Field"]) != row["Value"];
        }

        private static Difference CreateDifferenceForThisRow<T>(T instance, TableRow row)
        {
            if (ThePropertyDoesNotExist(instance, row))
                return new Difference
                           {
                               Property = row["Field"],
                               DoesNotExist = true
                           };

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
            public bool DoesNotExist { get; set; }
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