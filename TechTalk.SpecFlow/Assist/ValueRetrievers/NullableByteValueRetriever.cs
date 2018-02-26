using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableByteValueRetriever : IValueRetriever
    {
        private readonly Func<string, byte> byteValueRetriever = v => new ByteValueRetriever().GetValue(v);

        public NullableByteValueRetriever(Func<string, byte> byteValueRetriever = null)
        {
            if (byteValueRetriever != null)
                this.byteValueRetriever = byteValueRetriever;
        }

        public virtual byte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return byteValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(byte?);
        }
    }
}