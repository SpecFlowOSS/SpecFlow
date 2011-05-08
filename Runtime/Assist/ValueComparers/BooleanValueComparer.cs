using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class BooleanValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Boolean);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return Boolean.Parse(expectedValue) == (bool) actualValue;
        }
    }
}