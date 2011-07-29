using System;
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

        public SetComparer(Table table)
        {
            this.table = table;
        }

        public void CompareToSet(IEnumerable<T> set)
        {
            AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType();

            if (ThereAreNoResultsAndNoExpectedResults(set))
                return;

            if (ThereAreResultsWhenThereShouldBeNone(set))
                ThrowAnExpectedNoResultsError(set);

            AssertThatTheItemsMatchTheExpectedResults(set);

            AssertThatNoExtraRowsExist(set);
        }

        private void AssertThatNoExtraRowsExist(IEnumerable<T> set)
        {
            if (set.Count() != table.Rows.Count())
                throw new ComparisonException(string.Format(@"Expected {0} result{2}, but found {1}.",
                                                            table.Rows.Count(), set.Count(), table.Rows.Count() == 1 ? "" : "s"));
        }

        private static void ThrowAnExpectedNoResultsError(IEnumerable<T> set)
        {
            throw new ComparisonException(string.Format("There {0} {1} result{2} when expecting no results.",
                                                        set.Count() == 1 ? "was" : "were",
                                                        set.Count(),
                                                        set.Count() == 1 ? "" : "s"));
        }

        private bool ThereAreResultsWhenThereShouldBeNone(IEnumerable<T> set)
        {
            return set.Count() > 0 && table.Rows.Count() == 0;
        }

        private bool ThereAreNoResultsAndNoExpectedResults(IEnumerable<T> set)
        {
            return set.Count() == 0 && table.Rows.Count() == 0;
        }

        private void AssertThatTheItemsMatchTheExpectedResults(IEnumerable<T> set)
        {
            var listOfMissingItems = GetListOfExpectedItemsThatCouldNotBeFound(set);

            if (ExpectedItemsCouldNotBeFound(listOfMissingItems))
                ThrowAnErrorDetailingWhichItemsAreMissing(listOfMissingItems);
        }

        private IEnumerable<int> GetListOfExpectedItemsThatCouldNotBeFound(IEnumerable<T> set)
        {
            var actualItems = GetTheActualItems(set);

            var listOfMissingItems = new List<int>();

            var pivotTable = new PivotTable(table);

            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instanceTable = pivotTable.GetInstanceTable(index);

                var matchIndex = GetTheIndexOfTheMatchingItem(instanceTable, actualItems);

                if (matchIndex == MatchNotFound)
                    listOfMissingItems.Add(index + 1);
                else
                    RemoveFromActualItemsSoItWillNotBeCheckedAgain(actualItems, matchIndex);
            }
            return listOfMissingItems;
        }

        private static void ThrowAnErrorDetailingWhichItemsAreMissing(IEnumerable<int> listOfMissingItems)
        {
            throw new ComparisonException(
                listOfMissingItems.Aggregate(
                    @"The expected items at the following line numbers could not be matched:",
                    (running, next) => running + Environment.NewLine + next));
        }

        private static bool ExpectedItemsCouldNotBeFound(IEnumerable<int> listOfMissingItems)
        {
            return listOfMissingItems.Any();
        }

        private static void RemoveFromActualItemsSoItWillNotBeCheckedAgain(List<T> actualItems, int matchIndex)
        {
            actualItems.RemoveAt(matchIndex);
        }

        private static int GetTheIndexOfTheMatchingItem(Table expectedItem,
                                                 IList<T> actualItems)
        {
            for (var actualItemIndex = 0; actualItemIndex < actualItems.Count(); actualItemIndex++)
            {
                var actualItem = actualItems[actualItemIndex];

                if (ThisItemIsAMatch(expectedItem, actualItem))
                    return actualItemIndex;
            }
            return MatchNotFound;
        }

        private static bool ThisItemIsAMatch(Table expectedItem, T actualItem)
        {
            try
            {
                expectedItem.CompareToInstance(actualItem);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static List<T> GetTheActualItems(IEnumerable<T> set)
        {
            return set.ToList();
        }

        private void AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType()
        {
            var propertiesThatDoNotExist = from columnHeader in table.Header
                                           where (typeof(T).GetProperties().Any(property => TEHelpers.IsPropertyMatchingToColumnName(property, columnHeader)) == false)
                                           select columnHeader;

            if (propertiesThatDoNotExist.Any())
                throw new ComparisonException(
                    propertiesThatDoNotExist.Aggregate(@"The following fields do not exist:",
                                                       (running, next) => running + string.Format("{0}{1}", Environment.NewLine, next)));
        }
    }
}