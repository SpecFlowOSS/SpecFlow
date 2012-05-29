using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class FloatValueRetriever : IValueRetriever<float>
    {
        public virtual float GetValue(string value)
        {
            float returnValue = 0F;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out float result)
        {
            return float.TryParse(text, out result);
        }
    }
}