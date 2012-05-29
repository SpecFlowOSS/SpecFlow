using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DateTimeValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (DateTime);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            var valueRetriever = ValueRetrieverCollection.GetValueRetriever<DateTime>();
            var expected = valueRetriever.GetValue(expectedValue);

            if (expected == DateTime.MinValue) return false;
            return expected == (DateTime) actualValue;
        }
    }
}