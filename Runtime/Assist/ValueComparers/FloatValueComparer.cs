using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class FloatValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (float);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            float expected;
            if (float.TryParse(expectedValue, out expected) == false)
                return false;
            return expected == (float) actualValue;
        }
    }
}