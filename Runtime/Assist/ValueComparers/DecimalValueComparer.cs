using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class DecimalValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is decimal;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            Decimal expected;
            if (Decimal.TryParse(expectedValue, out expected) == false)
                return false;
            return expected == (decimal) actualValue;
        }
    }
}