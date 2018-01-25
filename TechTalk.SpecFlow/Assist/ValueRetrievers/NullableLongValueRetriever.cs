using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableLongValueRetriever : IValueRetriever
    {
        private readonly Func<string, long> longValueRetriever = v => new LongValueRetriever().GetValue(v);

        public NullableLongValueRetriever(Func<string, long> longValueRetriever = null)
        {
            if (longValueRetriever != null)
                this.longValueRetriever = longValueRetriever;
        }

        public virtual long? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return longValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(long?);
        }
    }
}