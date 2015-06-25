using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : ValueRetrieverBase
    {
        public virtual double GetValue(string value)
        {
            double returnValue = 0;
            Double.TryParse(value, out returnValue);
            return returnValue;
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(double) };
        }
    }
}