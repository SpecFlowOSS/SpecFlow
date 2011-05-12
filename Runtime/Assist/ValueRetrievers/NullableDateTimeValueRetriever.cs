using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableDateTimeValueRetriever
    {
        private readonly Func<string, DateTime> dateTimeValueRetriever;

        public NullableDateTimeValueRetriever(Func<string, DateTime> dateTimeValueRetriever)
        {
            this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        public DateTime? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeValueRetriever(value);
        }
    }
}