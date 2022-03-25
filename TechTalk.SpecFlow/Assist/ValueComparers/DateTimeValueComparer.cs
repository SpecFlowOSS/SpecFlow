using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class DateTimeValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is DateTime;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            return DateTime.TryParse(expectedValue, out var expected) &&
                   expected == (DateTime)actualValue;
        }
    }
}