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
                    var expectedItem = expectedItems[expectedItemIndex];
                    var actualItem = actualItems[actualItemIndex];

                    var propertiesToTest = GetAllPropertiesToTest(table);

                    var thisItemIsNotAMatch = ThisItemIsNotAMatch(propertiesToTest, expectedItem, actualItem);

                    if (thisItemIsNotAMatch == false && indexOfItemToRemove == -1)
                        indexOfItemToRemove = actualItemIndex;

                    if (thisItemIsNotAMatch == false)
                        successfulCheck = true;
                }
                if (successfulCheck == false)
                    throw new ComparisonException("");
                actualItems.RemoveAt(indexOfItemToRemove);
            }
        }

        private static bool ThisItemIsNotAMatch<T>(IEnumerable<string> propertiesToTest, T expectedItem, T actualItem)
        {
            var thisItemDoesNotMatchTheExpectedItem = false;
            foreach (var propertyName in propertiesToTest)
            {
                var expectedValue = expectedItem.GetPropertyValue(propertyName);
                var actualValue = actualItem.GetPropertyValue(propertyName);

                if (TheseValuesDoNotMatch(actualValue, expectedValue))
                    thisItemDoesNotMatchTheExpectedItem = true;
            }
            return thisItemDoesNotMatchTheExpectedItem;
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