using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class LongValueRetriever : IValueRetriever
    {
        public virtual long GetValue(string value)
        {
            long returnValue;
            long.TryParse(value, out returnValue);
            return returnValue;
        }
            
        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type)
        {
            return type == typeof(long);
        }
    }
}