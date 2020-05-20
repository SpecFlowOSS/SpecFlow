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
            Register(new UriValueRetriever());

            Register(new ArrayValueRetriever());
            Register(new ListValueRetriever());
        }
    }
}