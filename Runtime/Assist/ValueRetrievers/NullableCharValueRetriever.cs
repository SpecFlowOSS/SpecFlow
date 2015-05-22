using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever : IValueRetriever
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