using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class IntValueRetriever : ValueRetrieverBase
    {
        public virtual int GetValue(string value)
        {
            int returnValue;
            int.TryParse(value, out returnValue);
            return returnValue;
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(int);
        }
    }
}