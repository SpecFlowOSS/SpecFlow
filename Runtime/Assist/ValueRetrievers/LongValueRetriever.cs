﻿namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class LongValueRetriever : IValueRetriever<long>
    {
        public virtual long GetValue(string value)
        {
            long returnValue;
            long.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}