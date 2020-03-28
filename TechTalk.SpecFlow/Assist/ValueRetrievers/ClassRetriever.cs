using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public abstract class ClassRetriever<T> : IValueRetriever
        where T : class
    {
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(T);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            var value = keyValuePair.Value;
            if (value is null)
            {
                return default(T);
            }

            return GetNonEmptyValue(value);
        }

        protected abstract T GetNonEmptyValue(string value);
    }
}
