using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableByteValueRetriever : IValueRetriever<byte?>
    {
        private readonly IValueRetriever<byte> byteValueRetriever;

        public NullableByteValueRetriever(IValueRetriever<byte> byteValueRetriever)
        {
            this.byteValueRetriever = byteValueRetriever;
        }

        public byte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return byteValueRetriever.GetValue(value);
        }
    }
}