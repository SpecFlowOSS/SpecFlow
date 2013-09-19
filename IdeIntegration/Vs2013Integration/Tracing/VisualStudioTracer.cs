using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.Options;
using TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow;

namespace TechTalk.SpecFlow.Vs2010Integration.Tracing
{
    [Export(typeof(IVisualStudioTracer))]
    internal class VisualStudioTracer : IVisualStudioTracer
    {
        private const string TracingCategory = "Tracing";

        private IOutputWindowService outputWindowService;
        private Dispatcher dispatcher;

        [Import]
        public IIntegrationOptionsProvider IntegrationOptionsProvider { get; set; }

        [Import]
        public IOutputWindowService OutputWindowService
        {
            get { return outputWindowService; }
            set
            {
                outputWindowService = value;
                if (value != null)
                    Initialize();
            }
        }

        private DateTime configLastChecked = DateTime.MinValue;
        private TraceConfiguration traceConfiguration = new TraceConfiguration(false, "all");

        public VisualStudioTracer(IOutputWindowService outputWindowService) : this()
        {
            this.OutputWindowService = outputWindowService;
        }

        public VisualStudioTracer()
        {
            CheckConfiguration();
        }

        private class TraceConfiguration
        {
            private readonly string tracingCategoriesString;

            public bool Enabled { get; set; }
            public bool TraceAll { get; set; }
            public string[] TracingCategories { get; set; }

            public TraceConfiguration(IntegrationOptions integrationOptions) : this(integrationOptions.EnableTracing, integrationOptions.TracingCategories)
            {
                
            }

            public TraceConfiguration(bool enabled, string tracingCategories)
            {
                this.tracingCategoriesString = tracingCategories;
                Enabled = enabled;
                TraceAll =
                    string.Equals(tracingCategories, "all", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(tracingCategories, "true", StringComparison.InvariantCultureIgnoreCase);

                if (!TraceAll)
                {
                    Debug.Assert(tracingCategories != null);
                    TracingCategories = tracingCategories.Split(',').Select(cat => cat.Trim().ToLower()).ToArray();
                }
                else
                {
                    TracingCategories = null;
                }
            }

            public bool HasChanged(IntegrationOptions integrationOptions)
            {
                return integrationOptions.EnableTracing != Enabled ||
                       integrationOptions.TracingCategories != tracingCategoriesString;
            }
        }

        private void CheckConfiguration()
        {
            if (IntegrationOptionsProvider == null)
                return;

            var now = DateTime.Now;
            if (now - configLastChecked < TimeSpan.FromSeconds(10))
                return;

            var integrationOptions = IntegrationOptionsProvider.GetOptions();
            if (traceConfiguration.HasChanged(integrationOptions))
            {
                traceConfiguration = new TraceConfiguration(integrationOptions);
                OnTraceConfigChanged();
            }

            configLastChecked = now;
        }

        private void OnTraceConfigChanged()
        {
            if (!traceConfiguration.Enabled)
                return;

            if (traceConfiguration.TraceAll)
                Trace("Tracing enabled for all categories", TracingCategory);
            else
                Trace("Tracing enabled for categories: " + string.Join(", ", traceConfiguration.TracingCategories), TracingCategory);
        }

        private void Initialize()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Trace(string message, string category)
        {
            CheckConfiguration();

            if (!IsEnabled(category) || dispatcher == null)
                return;

            DateTime now = DateTime.Now;
            dispatcher.BeginInvoke(new Action(() =>
                {
                    var outputWindow =
                        OutputWindowService.TryGetPane(OutputWindowDefinitions.SpecFlowOutputWindowName);
                    if (outputWindow != null)
                        outputWindow.WriteLine(string.Format("[{2}] {0}: {1}", category, message, now.TimeOfDay));
                }), DispatcherPriority.ContextIdle);
        }

        public bool IsEnabled(string category)
        {
            if (!traceConfiguration.Enabled)
                return false;

            if (!traceConfiguration.TraceAll && !traceConfiguration.TracingCategories.Contains(category.ToLower()) && !category.Equals(TracingCategory, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        [Conditional("DEBUG")]
        static public void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException("Assertion fauiled: " + message);
        }
    }

}
