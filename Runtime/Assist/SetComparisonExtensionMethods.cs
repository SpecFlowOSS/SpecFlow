using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType<T>(table);

            AssertThatTheTableAndSetHaveTheSameNumberOfRows(table, set);

            if (ThereAreNoItems(set))
                return;

            var expectedItems = GetTheExpectedItems<T>(table);
            var actualItems = GetTheActualItems(set);

            for (var expectedItemIndex = 0; expectedItemIndex < expectedItems.Count(); expectedItemIndex++)
            {
                var successfulCheck = false;
                var indexOfItemToRemove = -1;
                for(var actualItemIndex = 0; actualItemIndex < actualItems.Count(); actualItemIndex++)
                {
                    var thisItemDoesNotMatchTheExpectedItem = false;
                    foreach (var property in GetAllPropertiesToTest(table))
                    {
                        var expectedValue = GetTheExpectedValue(expectedItemIndex, expectedItems, property);
                        var actualValue = GetTheActualValue(actualItemIndex, actualItems, property);

                        if (TheseValuesDoNotMatch(actualValue, expectedValue))
                            thisItemDoesNotMatchTheExpectedItem = true;

                        if (thisItemDoesNotMatchTheExpectedItem == false && indexOfItemToRemove == -1)
                            indexOfItemToRemove = actualItemIndex;
                    }
                    if (thisItemDoesNotMatchTheExpectedItem == false)
                        successfulCheck = true;
                }
                if (successfulCheck == false)
                    throw new ComparisonException("");
                actualItems.RemoveAt(indexOfItemToRemove);
            }
        }

        private static object GetTheActualValue<T>(int actualItemIndex, List<T> actualItems, string fieldToCheck)
        {
            return actualItems[actualItemIndex].GetPropertyValue(fieldToCheck);
        }

        private static object GetTheExpectedValue<T>(int expectedItemIndex, List<T> expectedItems, string fieldToCheck)
        {
            return expectedItems[expectedItemIndex].GetPropertyValue(fieldToCheck);
        }

        private static IEnumerable<string> GetAllPropertiesToTest(Table table)
        {
            return table.Header;
        }

        private static bool TheseValuesDoNotMatch(object actualValue, object expectedValue)
        {
            return (actualValue != null && expectedValue == null) || 
                   (actualValue == null && expectedValue != null) ||
                   ((actualValue != null && expectedValue != null) && (actualValue.ToString() != expectedValue.ToString()));
        }

        private static List<T> GetTheActualItems<T>(IEnumerable<T> set)
        {
            return set.ToList();
        }

        private static List<T> GetTheExpectedItems<T>(Table table)
        {
            return table.CreateSet<T>().ToList();
        }

        private static bool ThereAreNoItems<T>(IEnumerable<T> set)
        {
            return set.Count() == 0;
        }

        private static void AssertThatTheTableAndSetHaveTheSameNumberOfRows<T>(Table table, IEnumerable<T> set)
        {
            if (set.Count() != table.Rows.Count())
                throw new ComparisonException("");
        }

        private static void AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType<T>(Table table)
        {
            foreach (var id in table.Header)
                if (typeof (T).GetProperties().Select(x => x.Name).Contains(id) == false)
                    throw new ComparisonException("");
        }
    }
}