using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever : IValueRetriever
    {
        private readonly Func<string, char> charValueRetriever = v => new CharValueRetriever().GetValue(v);

        public NullableCharValueRetriever(Func<string, char> charValueRetriever = null)
        {
            if (charValueRetriever != null)
                this.charValueRetriever = charValueRetriever;
        }

        public virtual char? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return charValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(char?);
        }
    }
}