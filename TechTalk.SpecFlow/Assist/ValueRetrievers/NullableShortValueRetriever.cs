using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableShortValueRetriever : IValueRetriever
    {
        private readonly Func<string, short> shortValueRetriever = v => new ShortValueRetriever().GetValue(v);

        public NullableShortValueRetriever(Func<string, short> shortValueRetriever = null)
        {
            if (shortValueRetriever != null)
                this.shortValueRetriever = shortValueRetriever;
        }

        public virtual short? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return shortValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(short?);
        }
    }
}