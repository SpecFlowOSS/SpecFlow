using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        private const int MatchNotFound = -1;

        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType<T>(table);

            AssertThatTheTableAndSetHaveTheSameNumberOfRows(table, set);

            if (ThereAreNoItems(set))
                return;

            AssertThatTheItemsMatchTheExpectedResults(table, set);
        }

        private static void AssertThatTheItemsMatchTheExpectedResults<T>(Table table, IEnumerable<T> set)
        {
            var expectedItems = GetTheExpectedItems<T>(table);
            var actualItems = GetTheActualItems(set);

            var propertiesToTest = GetAllPropertiesToTest(table);

            foreach (var expectedItem in expectedItems)
                AssertThatAnActualitemMatchesThisExpectedItem(expectedItem, actualItems, propertiesToTest);
        }

        private static void AssertThatAnActualitemMatchesThisExpectedItem<T>(T expectedItem, List<T> actualItems,
                                                                             IEnumerable<string> propertiesToTest)
        {
            var matchIndex = GetTheIndexOfTheMatchingItem(expectedItem, actualItems, propertiesToTest);

            if (matchIndex == MatchNotFound)
                ThrowAComparisonException();

            RemoveFromActualItemsSoItWillNotBeCheckedAgain(actualItems, matchIndex);
        }

        private static void RemoveFromActualItemsSoItWillNotBeCheckedAgain<T>(List<T> actualItems, int matchIndex)
        {
            actualItems.RemoveAt(matchIndex);
        }

        private static void ThrowAComparisonException()
        {
            throw new ComparisonException("");
        }

        private static int GetTheIndexOfTheMatchingItem<T>(T expectedItem,
                                                           List<T> actualItems,
                                                           IEnumerable<string> propertiesToTest)
        {
            for (var actualItemIndex = 0; actualItemIndex < actualItems.Count(); actualItemIndex++)
            {
                var actualItem = actualItems[actualItemIndex];

                if (ThisItemIsAMatch(propertiesToTest, expectedItem, actualItem))
                    return actualItemIndex;
            }
            return MatchNotFound;
        }

        private static bool ThisItemIsAMatch<T>(IEnumerable<string> propertiesToTest, T expectedItem, T actualItem)
        {
            foreach (var propertyName in propertiesToTest)
            {
                var expectedValue = expectedItem.GetPropertyValue(propertyName);
                var actualValue = actualItem.GetPropertyValue(propertyName);

                if (TheseValuesDoNotMatch(actualValue, expectedValue))
                    return false;
            }
            return true;
        }

        private static IEnumerable<string> GetAllPropertiesToTest(Table table)
        {
            return table.Header;
        }

        private static bool TheseValuesDoNotMatch(object actualValue, object expectedValue)
        {
            return (actualValue != null && expectedValue == null) ||
                   (actualValue == null && expectedValue != null) ||
                   ((actualValue != null && expectedValue != null) &&
                    (actualValue.ToString() != expectedValue.ToString()));
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
                ThrowAComparisonException();
        }

        private static void AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType<T>(Table table)
        {
            foreach (var id in table.Header)
                if (typeof (T).GetProperties().Select(x => x.Name).Contains(id) == false)
                    ThrowAComparisonException();
        }
    }
}