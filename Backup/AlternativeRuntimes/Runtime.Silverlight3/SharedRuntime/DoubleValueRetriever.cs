using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class DoubleValueRetriever
    {
        public virtual double GetValue(string value)
        {
            double returnValue = 0;
            Double.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}