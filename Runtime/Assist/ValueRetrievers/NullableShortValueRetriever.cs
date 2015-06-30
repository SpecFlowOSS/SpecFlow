using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableShortValueRetriever : ValueRetrieverBase
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

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(short?);
        }
    }
}