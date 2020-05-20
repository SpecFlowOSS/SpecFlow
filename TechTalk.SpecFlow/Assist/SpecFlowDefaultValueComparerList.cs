using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.Assist
{
    public class SpecFlowDefaultValueComparerList : ServiceComponentList<IValueComparer>
    {
        public SpecFlowDefaultValueComparerList()
        {
            Register(new DateTimeValueComparer());
            Register(new BoolValueComparer());
            Register(new GuidValueComparer());
            Register(new DecimalValueComparer());
            Register(new DoubleValueComparer());
            Register(new FloatValueComparer());
            SetDefault(new DefaultValueComparer());
        }
    }
}