using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DecimalValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (decimal);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            Decimal expected;
            if (Decimal.TryParse(expectedValue, out expected) == false)
                return false;
            return expected == (decimal) actualValue;
        }
    }
}