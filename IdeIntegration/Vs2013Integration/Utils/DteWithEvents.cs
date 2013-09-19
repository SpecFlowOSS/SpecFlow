using System;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    /// <summary>
    /// This class is ncecessary because of COM interop. If the .NET wrapper of the event sources are 
    /// not referecned, the subscriptions might get lost.
    /// </summary>
    internal class DteWithEvents
    {
        public event _dispBuildEvents_OnBuildDoneEventHandler OnBuildDone
        {
            add
            {
                try
                {
                    BuildEvents.Value.OnBuildDone += value;
                }
                catch (Exception e)
                {
                    tracer.Trace("OnBuildDone: {0}", this, e);
                }
            }
            remove
            {
                try
                {
                    BuildEvents.Value.OnBuildDone -= value;
                }
                catch (Exception e)
                {
                    tracer.Trace("OnBuildDone: {0}", this, e);
                }
            }
        }

        public readonly DTE DTE;
        private readonly IIdeTracer tracer;
        public readonly SolutionEvents SolutionEvents;
        public readonly ProjectItemsEvents ProjectItemsEvents;
        public readonly DocumentEvents DocumentEvents;
        private readonly Lazy<BuildEvents> BuildEvents;
        public readonly CodeModelEvents CodeModelEvents;

        public readonly SolutionEventsListener SolutionEventsListener;
        public readonly FileChangeEventsListener FileChangeEventsListener;

        public DteWithEvents(DTE dte, IIdeTracer tracer)
        {
            DTE = dte;
            this.tracer = tracer;
            SolutionEvents = dte.Events.SolutionEvents;
            ProjectItemsEvents = ((Events2)dte.Events).ProjectItemsEvents;
            DocumentEvents = ((Events2) dte.Events).DocumentEvents;
            BuildEvents = new Lazy<BuildEvents>(() => ((Events2)dte.Events).BuildEvents, true);
            CodeModelEvents = ((Events2)dte.Events).CodeModelEvents;

            SolutionEventsListener = new SolutionEventsListener();
            FileChangeEventsListener = new FileChangeEventsListener(tracer);

            SolutionEvents.BeforeClosing += FileChangeEventsListener.StopListening;
        }
    }
}