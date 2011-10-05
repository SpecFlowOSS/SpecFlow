using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class GuidValueComparer : IValueComparer
    {
        private readonly GuidValueRetriever guidValueRetriever;

        public GuidValueComparer(GuidValueRetriever guidValueRetriever)
        {
            this.guidValueRetriever = guidValueRetriever;
        }

        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Guid);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            try
            {
                return new Guid(expectedValue) == (Guid) actualValue;
            }
            catch
            {
                var guid = guidValueRetriever.GetValue(expectedValue);
                if (guid == new Guid()) return false;
                return guid == (Guid)actualValue;
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