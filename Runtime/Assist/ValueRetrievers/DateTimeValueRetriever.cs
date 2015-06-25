using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeValueRetriever : ValueRetrieverBase
    {
        public virtual DateTime GetValue(string value)
        {
            var returnValue = DateTime.MinValue;
            DateTime.TryParse(value, out returnValue);
            return returnValue;
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(DateTime) };
        }
    }
}