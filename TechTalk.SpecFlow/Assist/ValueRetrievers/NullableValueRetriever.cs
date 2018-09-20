using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public abstract class NullableValueRetriever<TNullable> : IValueRetriever
    {
        bool IValueRetriever.CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(TNullable);
        }

        object IValueRetriever.Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public TNullable GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return default(TNullable);
            return GetNonEmptyValue(value);
        }

        protected abstract TNullable GetNonEmptyValue(string value);
    }
}
