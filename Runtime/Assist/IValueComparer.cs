using System;

namespace TechTalk.SpecFlow.Assist
{
    internal interface IValueComparer
    {
        bool CanCompare(object actualValue);
        bool TheseValuesAreTheSame(string expectedValue, object actualValue);
    }

    internal abstract class ValueComparerBase<T> : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue is T;
        }

        public abstract bool TheseValuesAreTheSame(string expectedValue, object actualValue);

        protected bool TheseValuesAreTheSame(string expectedValue, object actualValue, Func<object, object, bool> compare)
        {
            object value;
            if (!ScenarioContext.Current.ValueRetrievers.TryGetValue<T>(
                new ValueRetrieverContext(expectedValue), out value))
            {
                return false;
            }
            return compare(value, actualValue);
        }
    }
}