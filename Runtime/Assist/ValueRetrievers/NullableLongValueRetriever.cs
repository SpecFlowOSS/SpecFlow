using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableLongValueRetriever
    {
        private readonly Func<string, long> longValueRetriever;

        public NullableLongValueRetriever(Func<string, long> longValueRetriever)
        {
            this.longValueRetriever = longValueRetriever;
        }

        public long? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return longValueRetriever(value);
        }
    }
}