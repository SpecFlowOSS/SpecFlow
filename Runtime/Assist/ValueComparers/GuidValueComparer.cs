using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class GuidValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Guid);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            try
            {
                return new Guid(expectedValue) == (Guid) actualValue;
            }
            catch
            {
                return false;
            }
        }
    }
}