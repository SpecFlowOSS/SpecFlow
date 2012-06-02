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
            if(actualValue == null && string.IsNullOrEmpty(expectedValue))
                return true;
            if(actualValue == null) return false;

            var type = actualValue.GetType();

            object value;
            if (!ScenarioContext.Current.ValueRetrievers.TryGetValue(type,
                new ValueRetrieverContext(expectedValue), out value))
            {
                return false;
            }
            
            var equalsMethod = value.GetType().GetMethod("Equals", new[] {typeof (object)});
            return (bool) equalsMethod.Invoke(value, new[] {actualValue});
        }
    }
}