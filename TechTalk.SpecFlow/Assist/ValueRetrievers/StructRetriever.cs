using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public abstract class StructRetriever<T> : IValueRetriever
        where T : struct
    {
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(T) || propertyType == typeof(T?);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            var value = keyValuePair.Value;
            if (propertyType == typeof(T?) && string.IsNullOrEmpty(value))
            {
                return default(T?);
            }

            return GetNonEmptyValue(value);
        }

        protected abstract T GetNonEmptyValue(string value);
    }
}
