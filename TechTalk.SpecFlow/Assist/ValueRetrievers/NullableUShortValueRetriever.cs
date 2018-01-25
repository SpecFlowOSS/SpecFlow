using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUShortValueRetriever : IValueRetriever
    {
        private readonly Func<string, ushort> ushortValueRetriever = v => new UShortValueRetriever().GetValue(v);

        public NullableUShortValueRetriever(Func<string, ushort> ushortValueRetriever = null)
        {
            if (ushortValueRetriever != null)
                this.ushortValueRetriever = ushortValueRetriever;
        }

        public virtual ushort? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ushortValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(ushort?);
        }
    }
}