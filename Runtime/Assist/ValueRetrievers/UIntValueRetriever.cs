namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class UIntValueRetriever : ValueRetrieverBase<uint>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(uint.TryParse, context.Value, out result);
        }
    }
}