using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableLongValueRetriever : IValueRetriever<long?>
    {
        private readonly IValueRetriever<long> longValueRetriever;

        public NullableLongValueRetriever(IValueRetriever<long> longValueRetriever)
        {
            this.longValueRetriever = longValueRetriever;
        }

        public long? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return longValueRetriever.GetValue(value);
        }
    }
}