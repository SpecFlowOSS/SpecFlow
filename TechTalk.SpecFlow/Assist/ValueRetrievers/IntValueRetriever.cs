using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class IntValueRetriever : NonNullableValueRetriever<int>
    {
        public override int GetValue(string value)
        {
            int.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out int returnValue);
            return returnValue;
        }
    }
}