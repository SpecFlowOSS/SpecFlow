using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableBoolValueRetriever : IValueRetriever
    {
        private readonly Func<string, bool> boolValueRetriever = v => new BoolValueRetriever().GetValue(v);

        public NullableBoolValueRetriever(Func<string, bool> boolValueRetriever = null)
        {
            if (boolValueRetriever != null)
                this.boolValueRetriever = boolValueRetriever;
        }

        public virtual bool? GetValue(string thisValue)
        {
            if (string.IsNullOrEmpty(thisValue)) return null;
            return boolValueRetriever(thisValue);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(bool?);
        }
    }
}