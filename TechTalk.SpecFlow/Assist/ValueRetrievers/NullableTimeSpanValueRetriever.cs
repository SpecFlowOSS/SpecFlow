using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableTimeSpanValueRetriever : NullableValueRetriever<TimeSpan?>
    {
        private readonly Func<string, TimeSpan?> dateTimeValueRetriever = v => new TimeSpanValueRetriever().GetValue(v);

        public NullableTimeSpanValueRetriever(Func<string, TimeSpan?> dateTimeValueRetriever = null)
        {
            if (dateTimeValueRetriever != null)
                this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        protected override TimeSpan? GetNonEmptyValue(string value)
        {
            return dateTimeValueRetriever(value);
        }
    }
}