using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever : IValueRetriever
    {
        public virtual ushort GetValue(string value)
        {
            ushort returnValue;
            ushort.TryParse(value, out returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type propertyType)
        {
            return propertyType == typeof(ushort);
        }
    }
}