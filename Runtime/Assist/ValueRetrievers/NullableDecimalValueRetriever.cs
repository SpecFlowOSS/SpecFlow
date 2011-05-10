namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableDecimalValueRetriever
    {
        private readonly DecimalValueRetriever decimalValueRetriever;

        public NullableDecimalValueRetriever(DecimalValueRetriever decimalValueRetriever)
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