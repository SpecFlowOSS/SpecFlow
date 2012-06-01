namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class ShortValueRetriever : ValueRetrieverBase<short>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(short.TryParse, context.Value, out result);
        }
    }
}