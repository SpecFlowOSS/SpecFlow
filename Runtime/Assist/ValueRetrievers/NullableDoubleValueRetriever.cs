namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDoubleValueRetriever
    {
        private readonly DoubleValueRetriever DoubleValueRetriever;

        public NullableDoubleValueRetriever(DoubleValueRetriever DoubleValueRetriever)
        {
            this.DoubleValueRetriever = DoubleValueRetriever;
        }

        public double? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return DoubleValueRetriever.GetValue(value);
        }
    }
}