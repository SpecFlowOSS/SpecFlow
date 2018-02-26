using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableSByteValueRetriever : IValueRetriever
    {
        private readonly Func<string, sbyte> sbyteValueRetriever = v => new SByteValueRetriever().GetValue(v);

        public NullableSByteValueRetriever(Func<string, sbyte> sbyteValueRetriever = null)
        {
            if (sbyteValueRetriever != null)
                this.sbyteValueRetriever = sbyteValueRetriever;
        }

        public virtual sbyte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return sbyteValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(sbyte?);
        }
    }
}