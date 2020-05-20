using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class FloatValueRetriever : StructRetriever<float>
    {
        protected override float GetNonEmptyValue(string value)
        {
            float.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out float returnValue);
            return returnValue;
        }
    }
}