using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class FloatValueRetriever : NonNullableValueRetriever<float>
    {
        public override float GetValue(string value)
        {
            float.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out float returnValue);
            return returnValue;
        }
    }
}