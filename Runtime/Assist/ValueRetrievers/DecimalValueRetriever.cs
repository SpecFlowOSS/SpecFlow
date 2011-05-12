using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DecimalValueRetriever
    {
        public virtual decimal GetValue(string value)
        {
            var returnValue = 0M;
            Decimal.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}