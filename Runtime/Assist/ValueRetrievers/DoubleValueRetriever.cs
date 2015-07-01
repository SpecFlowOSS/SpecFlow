using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : IValueRetriever
    {
        public virtual double GetValue(string value)
        {
            double returnValue = 0;
            Double.TryParse(value, out returnValue);
            return returnValue;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(TableRow row, Type type)
        {
            return type == typeof(double);
        }
    }
}