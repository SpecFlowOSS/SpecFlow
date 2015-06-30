using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : ValueRetrieverBase
    {
        public virtual bool GetValue(string value)
        {
            return value == "True" || value == "true";
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(bool);
        }
    }
}