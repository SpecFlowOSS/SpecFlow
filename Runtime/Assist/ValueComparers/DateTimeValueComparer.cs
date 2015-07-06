using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class DateTimeValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (DateTime);
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            DateTime expected;
            if (DateTime.TryParse(expectedValue, out expected) == false)
                return false;
            return expected == (DateTime) actualValue;
        }
    }
}