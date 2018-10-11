using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public abstract class NonNullableValueRetriever<T> : IValueRetriever
    {
        bool IValueRetriever.CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(T);
        }

        object IValueRetriever.Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public abstract T GetValue(string value);
    }
}