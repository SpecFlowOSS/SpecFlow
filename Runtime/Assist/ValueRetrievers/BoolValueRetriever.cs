using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : IValueRetriever
    {
        public virtual bool GetValue(string value)
        {
            return value == "True" || value == "true";
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(TableRow row, Type type)
        {
            return type == typeof(bool);
        }
    }
}