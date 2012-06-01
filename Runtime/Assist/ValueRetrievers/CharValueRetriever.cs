namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class CharValueRetriever : ValueRetrieverBase<char>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(char.TryParse, context.Value, out result);
        }
    }
}