using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableShortValueRetriever : IValueRetriever
    {
        private readonly Func<string, short> shortValueRetriever = v => new ShortValueRetriever().GetValue(v);

        public NullableShortValueRetriever(Func<string, short> shortValueRetriever = null)
        {
            if (shortValueRetriever != null)
                this.shortValueRetriever = shortValueRetriever;
        }

        public short? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return shortValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(Type type)
        {
            return type == typeof(short?);
        }
    }
}