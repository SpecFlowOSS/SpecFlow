using System;

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
            return bool.Parse(expectedValue) == (bool) actualValue;
        }
    }
}