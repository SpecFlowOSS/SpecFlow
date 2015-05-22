using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableByteValueRetriever : IValueRetriever
    {
        private readonly Func<string, byte> byteValueRetriever;

        public NullableByteValueRetriever(Func<string, byte> byteValueRetriever)
        {
            this.byteValueRetriever = byteValueRetriever;
        }

        public byte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return byteValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }
    }
}