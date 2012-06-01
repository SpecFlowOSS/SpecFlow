namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class IntValueRetriever : ValueRetrieverBase<int>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(int.TryParse, context.Value, out result);
        }
    }
}