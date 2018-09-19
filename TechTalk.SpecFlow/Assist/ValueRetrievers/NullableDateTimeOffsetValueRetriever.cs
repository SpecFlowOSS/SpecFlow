using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDateTimeOffsetValueRetriever : NullableValueRetriever<DateTimeOffset?>
    {
        private readonly Func<string, DateTimeOffset> dateTimeOffsetValueRetriever;

        public NullableDateTimeOffsetValueRetriever()
            : this(v => new DateTimeOffsetValueRetriever().GetValue(v))
        {
        }

        public NullableDateTimeOffsetValueRetriever(Func<string, DateTimeOffset> dateTimeOffsetValueRetriever = null)
        {
            if (dateTimeOffsetValueRetriever != null)
                this.dateTimeOffsetValueRetriever = dateTimeOffsetValueRetriever;
        }

        protected override DateTimeOffset? GetNonEmptyValue(string value)
        {
            return dateTimeOffsetValueRetriever(value);
        }
    }
}