using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DateTimeOffsetValueRetriever
    {
        public virtual DateTimeOffset GetValue(string value)
        {
            var returnValue = DateTimeOffset.MinValue;
            DateTimeOffset.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}