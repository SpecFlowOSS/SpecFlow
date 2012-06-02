using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableShortValueRetriever : IValueRetriever<short?>
    {
        private readonly IValueRetriever<short> shortValueRetriever;

        public NullableShortValueRetriever(IValueRetriever<short> shortValueRetriever)
        {
            this.shortValueRetriever = shortValueRetriever;
        }

        public short? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return shortValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out short? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            short original;
            var tryResult = shortValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}