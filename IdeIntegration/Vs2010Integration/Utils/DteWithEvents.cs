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
        public readonly DTE DTE;
        public readonly SolutionEvents SolutionEvents;
        public readonly ProjectItemsEvents ProjectItemsEvents;
        public readonly DocumentEvents DocumentEvents;
        public readonly BuildEvents BuildEvents;
        public readonly CodeModelEvents CodeModelEvents;

        public readonly SolutionEventsListener SolutionEventsListener;
        public readonly FileChangeEventsListener FileChangeEventsListener;

        public DteWithEvents(DTE dte, IIdeTracer tracer)
        {
            DTE = dte;
            SolutionEvents = dte.Events.SolutionEvents;
            ProjectItemsEvents = ((Events2)dte.Events).ProjectItemsEvents;
            DocumentEvents = ((Events2) dte.Events).DocumentEvents;
            BuildEvents = ((Events2) dte.Events).BuildEvents;
            CodeModelEvents = ((Events2)dte.Events).CodeModelEvents;

            SolutionEventsListener = new SolutionEventsListener();
            FileChangeEventsListener = new FileChangeEventsListener(tracer);

            SolutionEvents.BeforeClosing += FileChangeEventsListener.StopListening;
        }
    }
}