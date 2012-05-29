using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DecimalValueRetriever : IValueRetriever<decimal>
    {
        public virtual decimal GetValue(string value)
        {
            decimal returnValue;
            Decimal.TryParse(value, out returnValue);
            return returnValue;
        }

        public virtual bool TryGetValue(string value, out decimal result)
        {
            return Decimal.TryParse(value, out result);
        }
    }
}