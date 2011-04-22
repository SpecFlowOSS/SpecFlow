using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever
    {
        public virtual double GetValue(string value)
        {
            double returnValue = 0;
            Double.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}