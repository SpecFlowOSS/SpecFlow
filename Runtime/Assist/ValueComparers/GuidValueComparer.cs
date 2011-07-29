using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class GuidValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Guid);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            expectedValue = AppendTrailingZeroesIfThisIsOnlyTheFirstEightCharactersOfAGuid(expectedValue);
            try
            {
                return new Guid(expectedValue) == (Guid) actualValue;
            }
            catch
            {
                return false;
            }
        }

        private static string AppendTrailingZeroesIfThisIsOnlyTheFirstEightCharactersOfAGuid(string expectedValue)
        {
            if (expectedValue.Length == 8)
                expectedValue += "-0000-0000-0000-000000000000";
            return expectedValue;
        }
    }
}