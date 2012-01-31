using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public static class InstanceComparisonExtensionMethods
    {
        public static void CompareToInstance<T>(this Table table, T instance)
        {
            AssertThatTheInstanceExists(instance);

            var instanceTable = TEHelpers.GetTheProperInstanceTable<T>(table);

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
                .Any(property => TEHelpers.IsPropertyMatchingToColumnName(property, row.Id())) == false;
        }

        private static bool TheValuesDoNotMatch<T>(T instance, TableRow row)
        {
            var expected = GetTheExpectedValue(row);
            var propertyValue = instance.GetPropertyValue(row.Id());

            var valueComparers = new IValueComparer[]
                                     {
                                         new DateTimeValueComparer(),
                                         new BoolValueComparer(),
                                         new GuidValueComparer(new GuidValueRetriever()),
                                         new DecimalValueComparer(),
                                         new DoubleValueComparer(),
                                         new FloatValueComparer(),
                                         new DefaultValueComparer()
                                     };

            return valueComparers
                .FirstOrDefault(x => x.CanCompare(propertyValue))
                .TheseValuesAreTheSame(expected, propertyValue) == false;
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