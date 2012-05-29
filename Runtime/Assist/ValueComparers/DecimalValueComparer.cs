using System;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DecimalValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (decimal);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            decimal expected;
            if (!ValueRetrieverCollection.GetValueRetriever<decimal>().TryGetValue(expectedValue, out expected))
                return false;
            return expected == (decimal) actualValue;
        }
    }
}