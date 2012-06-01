using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DecimalValueRetriever : ValueRetrieverBase<decimal>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(decimal.TryParse, context.Value, out result);
        }
    }
}