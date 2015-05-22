using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class SByteValueRetriever : IValueRetriever
    {
        public virtual sbyte GetValue(string value)
        {
            sbyte returnValue;
            sbyte.TryParse(value, out returnValue);
            return returnValue;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(sbyte) };
        }
    }
}