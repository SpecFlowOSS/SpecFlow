using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDateTimeValueRetriever : NullableValueRetriever<DateTime?>
    {
        private readonly Func<string, DateTime> dateTimeValueRetriever = v => new DateTimeValueRetriever().GetValue(v);

        public NullableDateTimeValueRetriever(Func<string, DateTime> dateTimeValueRetriever = null)
        {
            if (dateTimeValueRetriever != null)
                this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        protected override DateTime? GetNonEmptyValue(string value)
        {
            return dateTimeValueRetriever(value);
        }
    }
}