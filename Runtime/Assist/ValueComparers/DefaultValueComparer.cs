using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class DefaultValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return true;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            var actual = actualValue == null ? String.Empty : actualValue.ToString();

            return expectedValue == actual;
        }
    }
}