using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DecimalValueRetriever : ValueRetrieverBase
    {
        public virtual decimal GetValue(string value)
        {
            var returnValue = 0M;
            Decimal.TryParse(value, out returnValue);
            return returnValue;
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(decimal);
        }
    }
}