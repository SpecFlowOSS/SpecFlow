using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class SByteValueRetriever : ValueRetrieverBase
    {
        public virtual sbyte GetValue(string value)
        {
            sbyte returnValue;
            sbyte.TryParse(value, out returnValue);
            return returnValue;
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(sbyte);
        }
    }
}