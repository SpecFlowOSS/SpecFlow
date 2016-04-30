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
        public static bool TheValuesDoNotMatch<T>(T instance, TableRow row)
        {
            var expected = GetTheExpectedValue(row);
            var propertyValue = instance.GetPropertyValue(row.Id());

            return Service.Instance.ValueComparers
                .FirstOrDefault(x => x.CanCompare(propertyValue))
                .Compare(expected, propertyValue) == false;
        }

        public static bool ThereIsADifference<T>(T instance, TableRow row)
        {
            return InstanceComparisonExtensionMethods.ThePropertyDoesNotExist(instance, row) || TheValuesDoNotMatch(instance, row);
        }

        private static string GetTheExpectedValue(TableRow row)
        {
            return row.Value().ToString();
        }
    }
}