using System.Diagnostics;
using System.Globalization;
#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif
using System.Threading;

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
        private static bool isCurrentDisabled = false;
        private static FeatureContext current;
        public static FeatureContext Current
        {
            get
            {
                if (isCurrentDisabled)
                    throw new SpecFlowException("The FeatureContext.Current static accessor cannot be used in multi-threaded execution. Try injecting the feature context to the binding class. See http://go.specflow.org/doc-multithreaded for details.");
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL FeatureContext");
                }
                return current;
            }
            internal set
            {
                if (!isCurrentDisabled)
                    current = value;
            }
        }

        internal static void DisableSingletonInstance()
        {
            isCurrentDisabled = true;
            Thread.MemoryBarrier();
            current = null;
        }
        #endregion

        public FeatureInfo FeatureInfo { get; private set; }
        public CultureInfo BindingCulture { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }
    }
}