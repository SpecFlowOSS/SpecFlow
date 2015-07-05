using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UIntValueRetriever : IValueRetriever
    {
        public virtual uint GetValue(string value)
        {
            uint returnValue;
            uint.TryParse(value, out returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type)
        {
            return type == typeof(uint);
        }
    }
}