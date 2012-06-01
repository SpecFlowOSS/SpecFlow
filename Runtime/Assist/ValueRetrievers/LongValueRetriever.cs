namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class LongValueRetriever : ValueRetrieverBase<long>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(long.TryParse, context.Value, out result);
        }
    }
}