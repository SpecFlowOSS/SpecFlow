using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableDecimalValueRetriever
    {
        private readonly Func<string, decimal> decimalValueRetriever;

        public NullableDecimalValueRetriever(Func<string, decimal> decimalValueRetriever)
        {
            this.decimalValueRetriever = decimalValueRetriever;
        }

        public decimal? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return decimalValueRetriever(value);
        }
    }
}