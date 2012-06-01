namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class ByteValueRetriever : ValueRetrieverBase<byte>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(byte.TryParse, context.Value, out result);
        }
    }
}