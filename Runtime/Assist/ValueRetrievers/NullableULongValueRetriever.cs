using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableULongValueRetriever
    {
        private readonly Func<string, ulong> ulongValueRetriever;

        public NullableULongValueRetriever(Func<string, ulong> ulongValueRetriever)
        {
            this.ulongValueRetriever = ulongValueRetriever;
        }

        public ulong? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ulongValueRetriever(value);
        }
    }
}