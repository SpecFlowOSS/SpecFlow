using System.Diagnostics;
using System.Globalization;
#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif
using System.Threading;
using BoDi;

namespace TechTalk.SpecFlow
{
    public class FeatureContext : SpecFlowContext
    {
        public FeatureContext(IObjectContainer featureContainer, FeatureInfo featureInfo)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            FeatureContainer = featureContainer;
            FeatureInfo = featureInfo;
            BindingCulture = FeatureInfo.Language;
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

        public FeatureInfo FeatureInfo { get; }
        public CultureInfo BindingCulture { get; internal set; }
        public IObjectContainer FeatureContainer { get; }
        internal Stopwatch Stopwatch { get; }

        private bool isDisposed = false;
        protected override void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true; //HACK: we need this flag, because the FeatureContainer is disposed by the featureContextManager of the IContextManager and while we dispose the container itself, the it will call the dispose on us again...
            base.Dispose();

            FeatureContainer.Dispose();
        }
    }
}