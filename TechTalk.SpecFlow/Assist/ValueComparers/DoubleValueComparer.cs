using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class DoubleValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is double;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            return double.TryParse(expectedValue, out var expected) &&
                   Math.Abs(expected - (double) actualValue) < double.Epsilon;
        }
    }
}