using System;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class BoolValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (bool);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            var expected = ValueRetrieverCollection.GetValueRetriever<bool>().GetValue(expectedValue);
            return expected == (bool) actualValue;
        }
    }
}