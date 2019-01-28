using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    internal class SpecFlowDefaultValueRetrieverList : ServiceComponentList<IValueRetriever>
    {
        public SpecFlowDefaultValueRetrieverList()
        {
            Register<StringValueRetriever>();
            Register<ByteValueRetriever>();
            Register<SByteValueRetriever>();
            Register<IntValueRetriever>();
            Register<UIntValueRetriever>();
            Register<ShortValueRetriever>();
            Register<UShortValueRetriever>();
            Register<LongValueRetriever>();
            Register<ULongValueRetriever>();
            Register<FloatValueRetriever>();
            Register<DoubleValueRetriever>();
            Register<DecimalValueRetriever>();
            Register<CharValueRetriever>();
            Register<BoolValueRetriever>();
            Register<DateTimeValueRetriever>();
            Register<GuidValueRetriever>();
            Register<EnumValueRetriever>();
            Register<TimeSpanValueRetriever>();
            Register<DateTimeOffsetValueRetriever>();
            Register<NullableGuidValueRetriever>();
            Register<NullableDateTimeValueRetriever>();
            Register<NullableBoolValueRetriever>();
            Register<NullableCharValueRetriever>();
            Register<NullableDecimalValueRetriever>();
            Register<NullableDoubleValueRetriever>();
            Register<NullableFloatValueRetriever>();
            Register<NullableULongValueRetriever>();
            Register<NullableByteValueRetriever>();
            Register<NullableSByteValueRetriever>();
            Register<NullableIntValueRetriever>();
            Register<NullableUIntValueRetriever>();
            Register<NullableShortValueRetriever>();
            Register<NullableUShortValueRetriever>();
            Register<NullableLongValueRetriever>();
            Register<NullableTimeSpanValueRetriever>();
            Register<NullableDateTimeOffsetValueRetriever>();

            Register<ArrayValueRetriever>();
            Register<ListValueRetriever>();
        }
    }
}