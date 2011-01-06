using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Options;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    [Export(typeof(IGherkinProcessorServices))]
    internal class GherkinProcessorServices : IGherkinProcessorServices
    {
        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        [Import]
        internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        internal IVisualStudioTracer VisualStudioTracer = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public SpecFlowProject GetSpecFlowProjectFromProject(Project project)
        {
            //TODO: cache
            var specFlowProject = DteProjectReader.LoadSpecFlowProjectFromDteProject(project);
            return specFlowProject;
        }

        public SpecFlowProject GetSpecFlowProjectFromFile(ITextBuffer textBuffer)
        {
            Project project = GetProject(textBuffer);
            if (project == null)
                return null;

            return GetSpecFlowProjectFromProject(project);
        }

        public IntegrationOptions GetOptions()
        {
            var dte = VsxHelper.GetDte(ServiceProvider);
            return IntegrationOptionsProvider.GetOptions(dte);
        }

        public Project GetProject(ITextBuffer textBuffer)
        {
            return VsxHelper.GetCurrentProject(textBuffer, AdaptersFactory, ServiceProvider);
        }

        public GherkinDialect GetGherkinDialect(ITextBuffer textBuffer)
        {
            return GetParsingResult(textBuffer).GherkinDialect;
        }

        public GherkinFileEditorInfo GetParsingResult(ITextBuffer textBuffer)
        {
            var parser = GetParser(textBuffer);
            return parser.EnsureParsingResult(textBuffer.CurrentSnapshot);
        }

        public GherkinFileEditorParser GetParser(ITextBuffer textBuffer)
        {
            var specFlowProject = GetSpecFlowProjectFromFile(textBuffer);

            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
                new GherkinFileEditorParser(textBuffer, ClassificationRegistry, VisualStudioTracer, specFlowProject));
        }
    }
}
