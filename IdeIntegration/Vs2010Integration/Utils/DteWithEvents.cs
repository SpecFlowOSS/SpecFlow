using System;
using EnvDTE;
using EnvDTE80;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
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

        public DteWithEvents(DTE dte)
        {
            DTE = dte;
            SolutionEvents = dte.Events.SolutionEvents;
            ProjectItemsEvents = ((Events2)dte.Events).ProjectItemsEvents;
            DocumentEvents = ((Events2) dte.Events).DocumentEvents;
            BuildEvents = ((Events2) dte.Events).BuildEvents;
            CodeModelEvents = ((Events2)dte.Events).CodeModelEvents;
        }
    }
}