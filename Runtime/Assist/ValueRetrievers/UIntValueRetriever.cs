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

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(Type type)
        {
            return type == typeof(uint);
        }
    }
}