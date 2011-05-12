using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableIntValueRetriever
    {
        private readonly Func<string, int> intValueRetriever;

        public NullableIntValueRetriever(Func<string, int> intValueRetriever)
        {
            this.intValueRetriever = intValueRetriever;
        }

        public int? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return intValueRetriever(value);
        }
    }
}