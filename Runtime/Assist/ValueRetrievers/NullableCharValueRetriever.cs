namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever
    {
        private readonly CharValueRetriever charValueRetriever;

        public NullableCharValueRetriever(CharValueRetriever charValueRetriever)
        {
            this.charValueRetriever = charValueRetriever;
        }

        public char? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return charValueRetriever.GetValue(value);
        }
    }
}