using System.Collections.Generic;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.Assist
{
    #nullable enable
    internal sealed class SpecFlowDefaultValueComparerList : ServiceComponentList<IValueComparer>
    {
        public SpecFlowDefaultValueComparerList()
            : base(new List<IValueComparer> {
                    new DateTimeValueComparer(),
                    new BoolValueComparer(),
                    new GuidValueComparer(),
                    new DecimalValueComparer(),
                    new DoubleValueComparer(),
                    new FloatValueComparer(),
                    new DefaultValueComparer(),
                }, true)
        {
        }
    }
}