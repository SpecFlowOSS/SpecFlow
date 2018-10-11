using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ByteValueRetriever : NonNullableValueRetriever<byte>
    {
        public override byte GetValue(string value)
        {
            byte.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out byte returnValue);
            return returnValue;
        }
    }
}