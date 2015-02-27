using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableDateTimeOffsetValueRetriever
    {
        private readonly Func<string, DateTimeOffset> dateTimeOffsetValueRetriever;

        public NullableDateTimeOffsetValueRetriever(Func<string, DateTimeOffset> dateTimeOffsetValueRetriever)
        {
            this.dateTimeOffsetValueRetriever = dateTimeOffsetValueRetriever;
        }

        public DateTimeOffset? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeOffsetValueRetriever(value);
        }
    }
}