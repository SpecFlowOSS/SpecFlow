using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class InstanceComparisonExtensionMethods
    {

        public static void CompareToInstance<T>(this Table table, T instance)
        {
            AssertThatTheInstanceExists(instance);

            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof(T));

            var differences = FindAnyDifferences(instanceTable, instance);

            if (ThereAreAnyDifferences(differences))
                ThrowAnExceptionThatDescribesThoseDifferences(differences);
        }

        private static void AssertThatTheInstanceExists<T>(T instance)
        {
            if (instance == null)
                throw new ComparisonException("The item to compare was null.");
        }

        private static void ThrowAnExceptionThatDescribesThoseDifferences(IEnumerable<Difference> differences)
        {
            throw new ComparisonException(CreateDescriptiveErrorMessage(differences));
        }

        private static string CreateDescriptiveErrorMessage(IEnumerable<Difference> differences)
        {
            return differences.Aggregate(@"The following fields did not match:",
                                         (sum, next) => sum + (Environment.NewLine + DescribeTheErrorForThisDifference(next)));
        }

        private static string DescribeTheErrorForThisDifference(Difference difference)
        {
            return difference.DoesNotExist 
                ? $"{difference.Property}: Property does not exist" 
                : $"{difference.Property}: Expected <{difference.Expected}>, Actual <{difference.Actual}>";
        }

        private static IEnumerable<Difference> FindAnyDifferences<T>(Table table, T instance)
        {
            return from row in table.Rows
                   where ThePropertyDoesNotExist(instance, row) || TheValuesDoNotMatch(instance, row)
                   select CreateDifferenceForThisRow(instance, row);
        }

        private static bool ThereAreAnyDifferences(IEnumerable<Difference> differences)
        {
            return differences.Any();
        }

        private static bool ThePropertyDoesNotExist<T>(T instance, TableRow row)
        {
            return instance.GetType().GetProperties()
                .Any(property => TEHelpers.IsMemberMatchingToColumnName(property, row.Id())) == false;
        }

        private static bool TheValuesDoNotMatch<T>(T instance, TableRow row)
        {
            var expected = GetTheExpectedValue(row);
            var propertyValue = instance.GetPropertyValue(row.Id());

            var valueComparers = Service.Instance.ValueComparers;

            return valueComparers
                .FirstOrDefault(x => x.CanCompare(propertyValue))
                .Compare(expected, propertyValue) == false;
        }

        private static string GetTheExpectedValue(TableRow row)
        {
            return row.Value().ToString();
        }

        private static Difference CreateDifferenceForThisRow<T>(T instance, TableRow row)
        {
            if (ThePropertyDoesNotExist(instance, row))
                return new Difference
                           {
                               Property = row.Id(),
                               DoesNotExist = true
                           };

            return new Difference
                       {
                           Property = row.Id(),
                           Expected = row.Value(),
                           Actual = instance.GetPropertyValue(row.Id())
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

    public static class TableHelpers
    {
        public static string Id(this TableRow row)
        {
            return row[0];
        }

        public static object Value(this TableRow row)
        {
            return row[1];
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