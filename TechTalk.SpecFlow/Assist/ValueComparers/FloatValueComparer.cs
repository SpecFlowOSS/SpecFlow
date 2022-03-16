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
            return float.TryParse(expectedValue, out var expected) &&
                   Math.Abs(expected - (float) actualValue) < float.Epsilon;
        }
    }
}