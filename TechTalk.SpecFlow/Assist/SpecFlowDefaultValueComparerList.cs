using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.Assist
{
    public class SpecFlowDefaultValueComparerList : ServiceComponentList<IValueComparer>
    {
        public SpecFlowDefaultValueComparerList()
        {
            Register<DateTimeValueComparer>();
            Register<BoolValueComparer>();
            Register<GuidValueComparer>();
            Register<DecimalValueComparer>();
            Register<DoubleValueComparer>();
            Register<FloatValueComparer>();
            ReplaceDefault<DefaultValueComparer>();
        }
    }
}