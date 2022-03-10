using System.Collections.Generic;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    #nullable enable
    internal sealed class SpecFlowDefaultValueRetrieverList : ServiceComponentList<IValueRetriever>
    {
        public SpecFlowDefaultValueRetrieverList()
            : base(new List<IValueRetriever> {
                // Sorted by likelihood
                new StringValueRetriever(),
                new IntValueRetriever(),
                new BoolValueRetriever(),
                new LongValueRetriever(),
                new FloatValueRetriever(),
                new DoubleValueRetriever(),
                new DateTimeValueRetriever(),
                new TimeSpanValueRetriever(),
                new GuidValueRetriever(),
                new EnumValueRetriever(),
                new ListValueRetriever(),
                new ArrayValueRetriever(),
                new ByteValueRetriever(),
                new SByteValueRetriever(),
                new UIntValueRetriever(),
                new ShortValueRetriever(),
                new UShortValueRetriever(),
                new ULongValueRetriever(),
                new DecimalValueRetriever(),
                new CharValueRetriever(),
                new DateTimeOffsetValueRetriever(),
                new UriValueRetriever()
            }, false)
        {
        }
    }
}