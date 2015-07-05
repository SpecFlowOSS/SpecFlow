using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDecimalValueRetriever : IValueRetriever
    {
        private readonly Func<string, decimal> decimalValueRetriever = v => new DecimalValueRetriever().GetValue(v);

        public NullableDecimalValueRetriever(Func<string, decimal> decimalValueRetriever = null)
        {
            if (decimalValueRetriever != null)
                this.decimalValueRetriever = decimalValueRetriever;
        }

        public decimal? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return decimalValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type)
        {
            return type == typeof(decimal?);
        }
    }
}