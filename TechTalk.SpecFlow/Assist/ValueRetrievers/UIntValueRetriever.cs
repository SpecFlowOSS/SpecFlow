using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UIntValueRetriever : NonNullableValueRetriever<uint>
    {
        public override uint GetValue(string value)
        {
            uint.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out uint returnValue);
            return returnValue;
        }
    }
}