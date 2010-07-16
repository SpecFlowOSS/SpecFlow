using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.VsIntegration.Common;
using VSLangProj80;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal abstract class EditorExtensionProviderBase
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        [Import]
        internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

        protected GherkinFileEditorParser GetParser(ITextBuffer buffer)
        {
            var specFlowProject = GetSpecFlowProject(buffer);

            return GherkinFileEditorParser.GetParser(buffer, ClassificationRegistry, specFlowProject);
        }

        private SpecFlowProject GetSpecFlowProject(ITextBuffer buffer)
        {
            Project currentProject = GetCurrentProject(buffer, AdaptersFactory, ServiceProvider);
            if (currentProject == null || !IsProjectSupported(currentProject))
                return null;

            try
            {
                return DteProjectReader.LoadSpecFlowProjectFromDteProject(currentProject);
            }
            catch
            {
                return null;
            }
        }

        private bool IsProjectSupported(Project project)
        {
            return
                project.FullName.EndsWith(".csproj") ||
                project.FullName.EndsWith(".vbproj");
//                kind.Equals(vsContextGuids.vsContextGuidVCSProject, StringComparison.InvariantCultureIgnoreCase) ||
//                kind.Equals(vsContextGuids.vsContextGuidVBProject, StringComparison.InvariantCultureIgnoreCase);
        }

        /*private static Project GetCurrentProject(ITextBuffer buffer, IVsEditorAdaptersFactoryService adaptersFactory)
        {
            IExtensibleObject bufferAdapter = adaptersFactory.GetBufferAdapter(buffer) as IExtensibleObject;
            if (bufferAdapter == null)
                return null;
            object ppDisp;
            bufferAdapter.GetAutomationObject("Document", null, out ppDisp);
            Document document = ppDisp as Document;
            if (document == null)
                return null;

            return document.ProjectItem.ContainingProject;
        }*/

        private static Project GetCurrentProject(ITextBuffer buffer, IVsEditorAdaptersFactoryService adaptersFactory, SVsServiceProvider serviceProvider)
        {
            IPersistFileFormat persistFileFormat = adaptersFactory.GetBufferAdapter(buffer) as IPersistFileFormat;
            if (persistFileFormat == null)
                return null;

            string ppzsFilename;
            uint iii;
            persistFileFormat.GetCurFile(out ppzsFilename, out iii);

            if (string.IsNullOrWhiteSpace(ppzsFilename))
                return null;

            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));

            ProjectItem prjItem = dte.Solution.FindProjectItem(ppzsFilename);

            return prjItem.ContainingProject;
        }
    }
}