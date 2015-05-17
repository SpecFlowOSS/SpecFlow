using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class FloatValueRetriever
    {
        public virtual float GetValue(string value)
        {
            float returnValue = 0F;
            float.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}