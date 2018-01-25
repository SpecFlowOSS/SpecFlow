using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableULongValueRetriever : IValueRetriever
    {
        private readonly Func<string, ulong> ulongValueRetriever = v => new ULongValueRetriever().GetValue(v);

        public NullableULongValueRetriever(Func<string, ulong> ulongValueRetriever = null)
        {
            if (ulongValueRetriever != null)
                this.ulongValueRetriever = ulongValueRetriever;
        }

        public virtual ulong? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ulongValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(ulong?);
        }
    }
}