using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ShortValueRetriever : IValueRetriever
    {
        public virtual short GetValue(string value)
        {
            short returnValue;
            short.TryParse(value, out returnValue);
            return returnValue;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(TableRow row, Type type)
        {
            return type == typeof(short);
        }
    }
}