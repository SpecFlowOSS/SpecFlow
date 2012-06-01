using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DoubleValueRetriever : ValueRetrieverBase<double>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(double.TryParse, context.Value, out result);
        }
    }
}