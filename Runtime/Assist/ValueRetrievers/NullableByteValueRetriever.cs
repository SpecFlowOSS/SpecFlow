using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableByteValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, byte> byteValueRetriever = v => new ByteValueRetriever().GetValue(v);

        public NullableByteValueRetriever(Func<string, byte> byteValueRetriever = null)
        {
            if (byteValueRetriever != null)
                this.byteValueRetriever = byteValueRetriever;
        }

        public byte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return byteValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(byte?) };
        }
    }
}