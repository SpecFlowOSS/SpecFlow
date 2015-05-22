using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableSByteValueRetriever : IValueRetriever
    {
        private readonly Func<string, sbyte> sbyteValueRetriever;

        public NullableSByteValueRetriever(Func<string, sbyte> sbyteValueRetriever)
        {
            this.sbyteValueRetriever = sbyteValueRetriever;
        }

        public sbyte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return sbyteValueRetriever(value);
        }
    }
}