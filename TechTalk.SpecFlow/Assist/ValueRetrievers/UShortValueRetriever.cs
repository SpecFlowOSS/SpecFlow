using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever : StructRetriever<ushort>
    {
        protected override ushort GetNonEmptyValue(string value)
        {
            ushort.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out ushort returnValue);
            return returnValue;
        }
    }
}