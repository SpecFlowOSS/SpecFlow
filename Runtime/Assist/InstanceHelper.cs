using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    internal static class InstanceHelper
    {
        public static bool ThePropertyDoesNotExist<T>(T instance, TableRow row)
        {
            return instance.GetType().GetProperties()
                .Any(property => TEHelpers.IsMemberMatchingToColumnName(property, row.Id())) == false;
        }

        public static bool TheValuesDoNotMatch<T>(T instance, TableRow row)
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

        public static bool ThereIsADifference<T>(T instance, TableRow row)
        {
            return ThePropertyDoesNotExist(instance, row) || TheValuesDoNotMatch(instance, row);
        }

        private static string GetTheExpectedValue(TableRow row)
        {
            return row.Value().ToString();
        }
    }
}
