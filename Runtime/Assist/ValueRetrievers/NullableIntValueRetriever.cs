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
    }
}