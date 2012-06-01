namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class SByteValueRetriever : ValueRetrieverBase<sbyte>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(sbyte.TryParse, context.Value, out result);
        }
    }
}