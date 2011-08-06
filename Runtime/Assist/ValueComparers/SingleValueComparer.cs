using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class SingleValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Single);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            Single expected;
            if (Single.TryParse(expectedValue, out expected) == false)
                return false;
            return expected == (Single) actualValue;
        }
    }
}