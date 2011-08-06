using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class SingleValueRetriever
    {
        public virtual Single GetValue(string value)
        {
            Single returnValue = 0F;
            Single.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}