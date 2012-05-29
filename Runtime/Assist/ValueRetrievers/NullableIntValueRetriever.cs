using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableIntValueRetriever : IValueRetriever<int?>
    {
        private readonly IValueRetriever<int> intValueRetriever;

        public NullableIntValueRetriever(IValueRetriever<int> intValueRetriever)
        {
            this.intValueRetriever = intValueRetriever;
        }

        public int? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return intValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out int? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            int original;
            var tryResult = intValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}