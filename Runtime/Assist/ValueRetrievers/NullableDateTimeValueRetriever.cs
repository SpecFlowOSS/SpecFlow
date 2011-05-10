using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableDateTimeValueRetriever
    {
        private readonly DateTimeValueRetriever dateTimeValueRetriever;

        public NullableDateTimeValueRetriever(DateTimeValueRetriever dateTimeValueRetriever)
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