using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDateTimeValueRetriever : IValueRetriever<DateTime?>
    {
        private readonly IValueRetriever<DateTime> dateTimeValueRetriever;

        public NullableDateTimeValueRetriever(IValueRetriever<DateTime> dateTimeValueRetriever)
        {
            this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        public DateTime? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeValueRetriever.GetValue(value);
        }
    }
}