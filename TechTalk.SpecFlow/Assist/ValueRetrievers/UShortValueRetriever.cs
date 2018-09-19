using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever : NonNullableValueRetriever<ushort>
    {
        public override ushort GetValue(string value)
        {
            ushort.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out ushort returnValue);
            return returnValue;
        }
    }
}