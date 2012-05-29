using System;
using System.Collections.Generic;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeValueRetriever : IValueRetriever<DateTime>
    {
        public virtual DateTime GetValue(string value)
        {
            DateTime returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out DateTime result)
        {
            return DateTime.TryParse(text, out result);
        }
    }
}