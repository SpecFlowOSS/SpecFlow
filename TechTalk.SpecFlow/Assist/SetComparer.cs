using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class SetComparer<T>
    {
        private const int MatchNotFound = -1;
        private readonly Table table;
        private List<Tuple<T, int?>> extraOrNonMatchingActualItems;
        private readonly ITableDiffExceptionBuilder<T> tableDiffExceptionBuilder;

        public SetComparer(Table table)
        {
            this.table = table;
            tableDiffExceptionBuilder = BuildTheTableDiffExceptionBuilder();
        }

        private SafetyTableDiffExceptionBuilder<T> BuildTheTableDiffExceptionBuilder()
        {
            var buildsAnExceptionMessageForDiffResults = new TableDiffExceptionBuilder<T>();
            var applySpecialFormattingToTheExceptionMessage = new FormattingTableDiffExceptionBuilder<T>(buildsAnExceptionMessageForDiffResults);
            var makeSureTheFormattingWorkAboveDoesNotResultInABadException = new SafetyTableDiffExceptionBuilder<T>(applySpecialFormattingToTheExceptionMessage);
            return makeSureTheFormattingWorkAboveDoesNotResultInABadException;
        }

        public void CompareToSet(IEnumerable<T> set, bool sequentialEquality)
        {
            AssertThatAllColumnsInTheTableMatchToPropertiesOnTheType();

            if (ThereAreNoResultsAndNoExpectedResults(set))
                return;

            if (ThereAreResultsWhenThereShouldBeNone(set))
                ThrowAnExpectedNoResultsError(set, GetListOfExpectedItemsThatCouldNotBeFound(set));

            AssertThatTheItemsMatchTheExpectedResults(set);

            AssertThatNoExtraRowsExist(set, GetListOfExpectedItemsThatCouldNotBeFound(set));
        }

        private void AssertThatNoExtraRowsExist(IEnumerable<T> set, IEnumerable<int> listOfMissingItems)
        {
            if (set.Count() == table.Rows.Count()) return;

            ThrowAnErrorDetailingWhichItemsAreMissing(listOfMissingItems);
        }

        private void ThrowAnExpectedNoResultsError(IEnumerable<T> set, IEnumerable<int> listOfMissingItems)
        {
            ThrowAnErrorDetailingWhichItemsAreMissing(listOfMissingItems);
        }

        private bool ThereAreResultsWhenThereShouldBeNone(IEnumerable<T> set)
        {
            return set.Any() && !table.Rows.Any();
        }

        private bool ThereAreNoResultsAndNoExpectedResults(IEnumerable<T> set)
        {
            return !set.Any() && !table.Rows.Any();
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

            extraOrNonMatchingActualItems = actualItems.Select(i => new Tuple<T, int?>(i, null)).ToList();
            return listOfMissingItems;
        }

        private void ThrowAnErrorDetailingWhichItemsAreMissing(IEnumerable<int> listOfMissingItems)
        {
            var message = tableDiffExceptionBuilder.GetTheTableDiffExceptionMessage(new TableDifferenceResults<T>(table, listOfMissingItems, extraOrNonMatchingActualItems));
            throw new ComparisonException("\r\n" + message);
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
            for (var actualItemIndex = 0; actualItemIndex < actualItems.Count; actualItemIndex++)
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
                return expectedItem.IsEquivalentToInstance(actualItem);
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
            var normalizedPropertyNames = new HashSet<string>(from property in typeof(T).GetProperties()
                                                              select TEHelpers.NormalizePropertyNameToMatchAgainstAColumnName(property.Name));
            var normalizedColumnNames = new HashSet<string>(from columnHeader in table.Header
                                                            select TEHelpers.NormalizePropertyNameToMatchAgainstAColumnName(TEHelpers.RemoveAllCharactersThatAreNotValidInAPropertyName(columnHeader)));

            var propertiesThatDoNotExist = normalizedColumnNames.Except(normalizedPropertyNames, StringComparer.OrdinalIgnoreCase).ToArray();

            if (propertiesThatDoNotExist.Any())
                throw new ComparisonException($@"The following fields do not exist:{Environment.NewLine}{string.Join(Environment.NewLine, propertiesThatDoNotExist)}");
        }
    }
}