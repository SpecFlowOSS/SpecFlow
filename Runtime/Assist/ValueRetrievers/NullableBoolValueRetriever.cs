namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableBoolValueRetriever
    {
        private readonly BoolValueRetriever boolValueRetriever;

        public NullableBoolValueRetriever(BoolValueRetriever boolValueRetriever)
        {
            this.boolValueRetriever = boolValueRetriever;
        }

        public bool? GetValue(string thisValue)
        {
            if (string.IsNullOrEmpty(thisValue)) return null;
            return boolValueRetriever.GetValue(thisValue);
        }
    }
}