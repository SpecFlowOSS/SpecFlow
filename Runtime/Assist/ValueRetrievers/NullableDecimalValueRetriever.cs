using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDecimalValueRetriever : IValueRetriever<decimal?>
    {
        private readonly IValueRetriever<decimal> decimalValueRetriever;

        public NullableDecimalValueRetriever(IValueRetriever<decimal> decimalValueRetriever)
        {
            this.decimalValueRetriever = decimalValueRetriever;
        }

        public decimal? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return decimalValueRetriever.GetValue(value);
        }
    }
}