using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ByteValueRetriever : StructRetriever<byte>
    {
        protected override byte GetNonEmptyValue(string value)
        {
            byte.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out byte returnValue);
            return returnValue;
        }
    }
}