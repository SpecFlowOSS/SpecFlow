using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDoubleValueRetriever : IValueRetriever
    {
        private readonly Func<string, double> DoubleValueRetriever = v => new DoubleValueRetriever().GetValue(v);

        public NullableDoubleValueRetriever(Func<string, double> DoubleValueRetriever = null)
        {
            if (DoubleValueRetriever != null)
                this.DoubleValueRetriever = DoubleValueRetriever;
        }

        public virtual double? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return DoubleValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(double?);
        }
    }
}