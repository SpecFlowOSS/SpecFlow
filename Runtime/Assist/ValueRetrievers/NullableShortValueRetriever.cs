using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableShortValueRetriever : IValueRetriever
    {
        private readonly Func<string, short> shortValueRetriever;

        public NullableShortValueRetriever(Func<string, short> shortValueRetriever)
        {
            this.shortValueRetriever = shortValueRetriever;
        }

        public short? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return shortValueRetriever(value);
        }
    }
}