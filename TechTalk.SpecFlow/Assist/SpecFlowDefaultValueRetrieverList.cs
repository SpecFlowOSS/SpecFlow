using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    internal class SpecFlowDefaultValueRetrieverList : ServiceComponentList<IValueRetriever>
    {
        public SpecFlowDefaultValueRetrieverList()
        {
            Register(new StringValueRetriever());
            Register(new ByteValueRetriever());
            Register(new SByteValueRetriever());
            Register(new IntValueRetriever());
            Register(new UIntValueRetriever());
            Register(new ShortValueRetriever());
            Register(new UShortValueRetriever());
            Register(new LongValueRetriever());
            Register(new ULongValueRetriever());
            Register(new FloatValueRetriever());
            Register(new DoubleValueRetriever());
            Register(new DecimalValueRetriever());
            Register(new CharValueRetriever());
            Register(new BoolValueRetriever());
            Register(new DateTimeValueRetriever());
            Register(new GuidValueRetriever());
            Register(new EnumValueRetriever());
            Register(new TimeSpanValueRetriever());
            Register(new DateTimeOffsetValueRetriever());
            Register(new NullableGuidValueRetriever());
            Register(new NullableDateTimeValueRetriever());
            Register(new NullableBoolValueRetriever());
            Register(new NullableCharValueRetriever());
            Register(new NullableDecimalValueRetriever());
            Register(new NullableDoubleValueRetriever());
            Register(new NullableFloatValueRetriever());
            Register(new NullableULongValueRetriever());
            Register(new NullableByteValueRetriever());
            Register(new NullableSByteValueRetriever());
            Register(new NullableIntValueRetriever());
            Register(new NullableUIntValueRetriever());
            Register(new NullableShortValueRetriever());
            Register(new NullableUShortValueRetriever());
            Register(new NullableLongValueRetriever());
            Register(new NullableTimeSpanValueRetriever());
            Register(new NullableDateTimeOffsetValueRetriever());

            Register(new StringArrayValueRetriever());
            Register(new StringListValueRetriever());
            Register(new EnumArrayValueRetriever());
            Register(new EnumListValueRetriever());
        }
    }
}