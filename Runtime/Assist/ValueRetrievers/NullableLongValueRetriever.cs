using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableLongValueRetriever : IValueRetriever
    {
        private readonly Func<string, long> longValueRetriever;

        public NullableLongValueRetriever(Func<string, long> longValueRetriever)
        {
            this.longValueRetriever = longValueRetriever;
        }

        public long? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return longValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(long?) };
        }
    }
}