namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableIntValueRetriever
    {
        private readonly IntValueRetriever intValueRetriever;

        public NullableIntValueRetriever(IntValueRetriever intValueRetriever)
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