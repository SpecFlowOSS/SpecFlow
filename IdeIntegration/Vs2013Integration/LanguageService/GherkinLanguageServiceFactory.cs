using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinLanguageServiceFactory
    {
        GherkinLanguageService GetLanguageService(ITextBuffer textBuffer);
    }

    [Export(typeof(IGherkinLanguageServiceFactory))]
    internal class GherkinLanguageServiceFactory : IGherkinLanguageServiceFactory
    {
        [Import]
        internal IProjectScopeFactory ProjectScopeFactory = null;

        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        [Import]
        internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        internal IVisualStudioTracer VisualStudioTracer = null;

        [Import]
        internal IGherkinBufferServiceManager GherkinBufferServiceManager = null;

        [Import]
        IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        internal IBiDiContainerProvider ContainerProvider = null;

        public GherkinLanguageService GetLanguageService(ITextBuffer textBuffer)
        {
            var gherkinLanguageService = GherkinBufferServiceManager.GetOrCreate(textBuffer, () => CreateLanguageService(textBuffer));
            gherkinLanguageService.EnsureInitialized(textBuffer);
            return gherkinLanguageService;
        }

        private GherkinLanguageService CreateLanguageService(ITextBuffer textBuffer)
        {
            var project = VsxHelper.GetCurrentProject(textBuffer, AdaptersFactory, ServiceProvider);
            var projectScope = ProjectScopeFactory.GetProjectScope(project);

            ContainerProvider.ObjectContainer.Resolve<InstallServices>().OnPackageUsed(); //TODO: find a better place

            var languageService = new GherkinLanguageService(projectScope, VisualStudioTracer, enableStepMatchColoring: IntegrationOptionsProvider.GetOptions().EnableStepMatchColoring);

            textBuffer.Changed +=
                (sender, args) => languageService.TextBufferChanged(GetTextBufferChange(args));

            return languageService;
        }

        private GherkinTextBufferChange GetTextBufferChange(TextContentChangedEventArgs textContentChangedEventArgs)
        {
            Tracing.VisualStudioTracer.Assert(textContentChangedEventArgs.Changes.Count > 0, "There are no text changes");

            var startLine = int.MaxValue;
            var endLine = 0;
            var startPosition = int.MaxValue;
            var endPosition = 0;
            var lineCountDelta = 0;
            var positionDelta = 0;

            var beforeTextSnapshot = textContentChangedEventArgs.Before;
            var afterTextSnapshot = textContentChangedEventArgs.After;
            foreach (var change in textContentChangedEventArgs.Changes)
            {
                startLine = Math.Min(startLine, beforeTextSnapshot.GetLineNumberFromPosition(change.OldPosition));
                endLine = Math.Max(endLine, afterTextSnapshot.GetLineNumberFromPosition(change.NewEnd));

                startPosition = Math.Min(startPosition, change.OldPosition);
                endPosition = Math.Max(endPosition, change.NewEnd);
                lineCountDelta += change.LineCountDelta;
                positionDelta += change.Delta;
            }

            return new GherkinTextBufferChange(
                startLine == endLine ? GherkinTextBufferChangeType.SingleLine : GherkinTextBufferChangeType.MultiLine,
                startLine, endLine,
                startPosition, endPosition,
                lineCountDelta, positionDelta,
                afterTextSnapshot);
        }
    }
}