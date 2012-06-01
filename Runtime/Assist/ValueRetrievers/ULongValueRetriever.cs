namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class ULongValueRetriever : ValueRetrieverBase<ulong>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(ulong.TryParse, context.Value, out result);
        }
    }
}