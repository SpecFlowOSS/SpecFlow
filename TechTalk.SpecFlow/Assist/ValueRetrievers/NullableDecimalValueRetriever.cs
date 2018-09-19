using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDecimalValueRetriever : NullableValueRetriever<decimal?>
    {
        private readonly Func<string, decimal> decimalValueRetriever = v => new DecimalValueRetriever().GetValue(v);

        public NullableDecimalValueRetriever(Func<string, decimal> decimalValueRetriever = null)
        {
            if (decimalValueRetriever != null)
                this.decimalValueRetriever = decimalValueRetriever;
        }

        protected override decimal? GetNonEmptyValue(string value)
        {
            return decimalValueRetriever(value);
        }
    }
}