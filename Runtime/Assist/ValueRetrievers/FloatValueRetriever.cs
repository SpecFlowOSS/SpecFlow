using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class FloatValueRetriever : ValueRetrieverBase<float>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(float.TryParse, context.Value, out result);
        }
    }
}