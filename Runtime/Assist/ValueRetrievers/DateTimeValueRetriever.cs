using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DateTimeValueRetriever : ValueRetrieverBase<DateTime>
    {
        public override bool TryGetValue(ValueRetrieverContext context, out object result)
        {
            return TryParse(DateTime.TryParse, context.Value, out result);
        }
    }
}