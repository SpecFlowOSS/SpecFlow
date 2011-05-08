using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DefaultValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return true;
        }

        public bool CompareValue(string expectedValue, object actualValue)
        {

            var actual = actualValue == null ? String.Empty : actualValue.ToString();

            if (ThisIsABooleanThatNeedsToBeLoweredToMatchAssistConventions(actualValue))
                actual = actual.ToLower();

            if (ThisIsAGuidThatNeedsToBeUppedToMatchToStringGuidValue(actualValue))
                actual = actual.ToUpper();

            return expectedValue == actual;
        }

        private static bool ThisIsAGuidThatNeedsToBeUppedToMatchToStringGuidValue(object propertyValue)
        {
            return propertyValue != null && propertyValue.GetType() == typeof(Guid);
        }

        private static bool ThisIsABooleanThatNeedsToBeLoweredToMatchAssistConventions(object propertyValue)
        {
            return propertyValue != null && propertyValue.GetType() == typeof(bool);
        }
    }
}