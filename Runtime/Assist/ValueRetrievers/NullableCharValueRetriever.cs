using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever : IValueRetriever<char?>
    {
        private readonly IValueRetriever<char> charValueRetriever;

        public NullableCharValueRetriever(IValueRetriever<char> charValueRetriever)
        {
            this.charValueRetriever = charValueRetriever;
        }

        public char? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return charValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out char? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            char original;
            var tryResult = charValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}