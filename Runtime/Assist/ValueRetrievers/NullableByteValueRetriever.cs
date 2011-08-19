using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableByteValueRetriever
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
    }
}