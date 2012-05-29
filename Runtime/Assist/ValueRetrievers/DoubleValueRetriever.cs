using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : IValueRetriever<double>
    {
        public virtual double GetValue(string value)
        {
            double returnValue = 0;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out double result)
        {
            return double.TryParse(text, out result);
        }
    }
}