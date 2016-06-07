using System.Diagnostics;
using System.Globalization;
using System.Reflection;
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

        #region Singleton
        public static FeatureContext Current => TestRunnerManager.GetTestRunner(Assembly.GetCallingAssembly()).FeatureContext;

        #endregion

        public FeatureInfo FeatureInfo { get; private set; }
        public CultureInfo BindingCulture { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }
    }
}