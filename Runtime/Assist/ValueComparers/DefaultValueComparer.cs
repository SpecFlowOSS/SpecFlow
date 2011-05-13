using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DefaultValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return true;
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            var actual = actualValue == null ? String.Empty : actualValue.ToString();

            return expectedValue == actual;
        }
    }
}