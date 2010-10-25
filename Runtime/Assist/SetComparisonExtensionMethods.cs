using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            var checker = new SetComparer<T>(table);
            checker.CompareToSet(set);
        }
    }

    public class SetComparer<T>
    {
        private const int MatchNotFound = -1;
        private readonly Table table;
        private readonly IEnumerable<string> propertiesToTest;
        private readonly List<T> expectedItems;

        public SetComparer(Table table)
        {
            this.table = table;
            propertiesToTest = GetAllPropertiesToTest(table);
            expectedItems = GetTheExpectedItems<T>(table);
        }

        public void CompareToSet(IEnumerable<T> set)
        {
            AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType();

            AssertThatTheTableAndSetHaveTheSameNumberOfRows(set);

            if (ThereAreNoItems(set))
                return;

            AssertThatTheItemsMatchTheExpectedResults(set);
        }

        private void AssertThatTheItemsMatchTheExpectedResults(IEnumerable<T> set)
        {
            var actualItems = GetTheActualItems(set);

            foreach (var expectedItem in expectedItems)
                AssertThatAnActualitemMatchesThisExpectedItem(expectedItem, actualItems);
        }

        private void AssertThatAnActualitemMatchesThisExpectedItem(T expectedItem, List<T> actualItems)
        {
            var matchIndex = GetTheIndexOfTheMatchingItem(expectedItem, actualItems);

            if (matchIndex == MatchNotFound)
                ThrowAComparisonException();

            RemoveFromActualItemsSoItWillNotBeCheckedAgain(actualItems, matchIndex);
        }

        private static void RemoveFromActualItemsSoItWillNotBeCheckedAgain(List<T> actualItems, int matchIndex)
        {
            actualItems.RemoveAt(matchIndex);
        }

        private static void ThrowAComparisonException()
        {
            throw new ComparisonException("");
        }

        private int GetTheIndexOfTheMatchingItem<T>(T expectedItem,
                                                    List<T> actualItems)
        {
            for (var actualItemIndex = 0; actualItemIndex < actualItems.Count(); actualItemIndex++)
            {
                var actualItem = actualItems[actualItemIndex];

                if (ThisItemIsAMatch(expectedItem, actualItem))
                    return actualItemIndex;
            }
            return MatchNotFound;
        }

        private bool ThisItemIsAMatch<T>(T expectedItem, T actualItem)
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

        private void AssertThatTheTableAndSetHaveTheSameNumberOfRows<T>(IEnumerable<T> set)
        {
            if (set.Count() != table.Rows.Count())
                ThrowAComparisonException();
        }

        private void AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType()
        {
            foreach (var id in table.Header)
                if (typeof (T).GetProperties().Select(x => x.Name).Contains(id) == false)
                    ThrowAComparisonException();
        }
    }
}