using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUIntValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, uint> uintValueRetriever = v => new UIntValueRetriever().GetValue(v);

        public NullableUIntValueRetriever(Func<string, uint> uintValueRetriever = null)
        {
            if (uintValueRetriever != null)
                this.uintValueRetriever = uintValueRetriever;
        }

        public uint? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return uintValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(uint?);
        }
    }
}