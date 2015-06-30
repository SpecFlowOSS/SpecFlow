using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUShortValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, ushort> ushortValueRetriever = v => new UShortValueRetriever().GetValue(v);

        public NullableUShortValueRetriever(Func<string, ushort> ushortValueRetriever = null)
        {
            if (ushortValueRetriever != null)
                this.ushortValueRetriever = ushortValueRetriever;
        }

        public ushort? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ushortValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(ushort?);
        }
    }
}