using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ByteValueRetriever : IValueRetriever
    {
        public virtual byte GetValue(string value)
        {
            byte returnValue;
            byte.TryParse(value, out returnValue);
            return returnValue;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(Type type)
        {
            return type == typeof(byte);
        }
    }
}