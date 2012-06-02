using System;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DoubleValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (double);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            Double expected;
            if (ValueRetrieverCollection.GetValueRetriever<double>().TryGetValue(expectedValue, out expected) == false)
                return false;
            return expected == (double) actualValue;
        }
    }
}