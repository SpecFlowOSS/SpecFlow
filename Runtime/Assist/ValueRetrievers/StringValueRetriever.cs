using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringValueRetriever : IValueRetriever
    {
        public string GetValue(string value)
        {
            return value;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }
    }
}