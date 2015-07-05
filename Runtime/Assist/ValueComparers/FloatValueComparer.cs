using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class FloatValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (float);
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            float expected;
            if (float.TryParse(expectedValue, out expected) == false)
                return false;
            return expected == (float) actualValue;
        }
    }
}