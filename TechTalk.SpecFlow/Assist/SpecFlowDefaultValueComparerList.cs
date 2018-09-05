using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public class SpecFlowDefaultValueComparerList : ServiceComponentList<IValueComparer>
    {
        public SpecFlowDefaultValueComparerList()
        {
            Register(new DateTimeValueComparer());
            Register(new BoolValueComparer());
            Register(new GuidValueComparer(new GuidValueRetriever()));
            Register(new DecimalValueComparer());
            Register(new DoubleValueComparer());
            Register(new FloatValueComparer());
            RegisterDefault(new DefaultValueComparer());
        }
    }
}