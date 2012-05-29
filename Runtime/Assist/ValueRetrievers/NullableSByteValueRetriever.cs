using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableSByteValueRetriever : IValueRetriever<sbyte?>
    {
        private readonly IValueRetriever<sbyte> sbyteValueRetriever;

        public NullableSByteValueRetriever(IValueRetriever<sbyte> sbyteValueRetriever)
        {
            this.sbyteValueRetriever = sbyteValueRetriever;
        }

        public sbyte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return sbyteValueRetriever.GetValue(value);
        }
    }
}