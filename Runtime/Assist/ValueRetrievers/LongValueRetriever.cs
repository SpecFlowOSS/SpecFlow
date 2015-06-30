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
            
        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(Type type)
        {
            return type == typeof(long);
        }
    }
}