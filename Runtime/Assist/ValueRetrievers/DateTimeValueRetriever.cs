using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DateTimeValueRetriever
    {
        public virtual DateTime GetValue(string value)
        {
            var returnValue = DateTime.MinValue;
            DateTime.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}