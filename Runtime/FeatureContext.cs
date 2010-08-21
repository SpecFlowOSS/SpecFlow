using System.Diagnostics;
using System.Globalization;

#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif

namespace TechTalk.SpecFlow
{
    public class FeatureContext : SpecFlowContext
    {
        public FeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            BindingCulture = bindingCulture;
            FeatureInfo = featureInfo;
        }

        public static FeatureContext Current
        {
            get { return ObjectContainer.FeatureContext; }
        }

        public FeatureInfo FeatureInfo { get; private set; }
        public CultureInfo BindingCulture { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }
    }
}