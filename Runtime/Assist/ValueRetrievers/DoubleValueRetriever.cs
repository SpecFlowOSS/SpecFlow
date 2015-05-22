using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : IValueRetriever
    {
        public virtual double GetValue(string value)
        {
            double returnValue = 0;
            Double.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}