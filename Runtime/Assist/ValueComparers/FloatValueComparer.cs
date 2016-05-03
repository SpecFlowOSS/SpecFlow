using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class FloatValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is float;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            float expected;
            if (float.TryParse(expectedValue, out expected) == false)
                return false;
            return Math.Abs(expected - (float) actualValue) < float.Epsilon;
        }
    }
}