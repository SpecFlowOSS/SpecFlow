namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class BoolValueRetriever : ValueRetrieverBase<bool>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(bool.TryParse, context.Value, out result);
        }
    }
}