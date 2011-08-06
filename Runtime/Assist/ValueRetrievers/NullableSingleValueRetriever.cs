using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableSingleValueRetriever
    {
        private readonly Func<string, Single> SingleValueRetriever;

        public NullableSingleValueRetriever(Func<string, Single> SingleValueRetriever)
        {
            this.SingleValueRetriever = SingleValueRetriever;
        }

        public Single? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return SingleValueRetriever(value);
        }
    }
}