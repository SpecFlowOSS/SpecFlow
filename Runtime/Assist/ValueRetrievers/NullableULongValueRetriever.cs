using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableULongValueRetriever : IValueRetriever<ulong?>
    {
        private readonly IValueRetriever<ulong> ulongValueRetriever;

        public NullableULongValueRetriever(IValueRetriever<ulong> ulongValueRetriever)
        {
            this.ulongValueRetriever = ulongValueRetriever;
        }

        public ulong? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ulongValueRetriever.GetValue(value);
        }
    }
}