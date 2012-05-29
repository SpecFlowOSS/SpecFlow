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

        public bool TryGetValue(string text, out long? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            long original;
            var tryResult = longValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}