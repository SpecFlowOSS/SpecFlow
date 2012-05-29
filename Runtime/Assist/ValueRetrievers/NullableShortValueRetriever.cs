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
    }
}