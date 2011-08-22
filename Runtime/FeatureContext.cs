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

        private static FeatureContext current;
        public static FeatureContext Current
        {
            get
            {
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL FeatureContext");
                }
                return current;
            }
            internal set { current = value; }
        }

        public FeatureInfo FeatureInfo { get; private set; }
        public CultureInfo BindingCulture { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }
    }
}