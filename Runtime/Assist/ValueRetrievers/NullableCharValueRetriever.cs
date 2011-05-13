using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableCharValueRetriever
    {
        private readonly Func<string, char> charValueRetriever;

        public NullableCharValueRetriever(Func<string, char> charValueRetriever)
        {
            this.charValueRetriever = charValueRetriever;
        }

        public char? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return charValueRetriever(value);
        }
    }
}