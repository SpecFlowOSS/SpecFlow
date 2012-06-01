namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class UShortValueRetriever : ValueRetrieverBase<ushort>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(ushort.TryParse, context.Value, out result);
        }
    }
}