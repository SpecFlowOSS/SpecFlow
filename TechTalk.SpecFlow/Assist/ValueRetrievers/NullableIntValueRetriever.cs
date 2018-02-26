using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableIntValueRetriever : IValueRetriever
    {
        private readonly Func<string, int> intValueRetriever = v => new IntValueRetriever().GetValue(v);

        public NullableIntValueRetriever(Func<string, int> intValueRetriever = null)
        {
            if (intValueRetriever != null)
                this.intValueRetriever = intValueRetriever;
        }

        public virtual int? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return intValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(int?);
        }
    }
}