using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Common;
using VSLangProj80;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal abstract class EditorExtensionProviderBase
    {
        [Import]
        internal IGherkinProcessorServices GherkinProcessorServices = null;

        protected GherkinFileEditorParser GetParser(ITextBuffer buffer)
        {
            return GherkinProcessorServices.GetParser(buffer);
        }
    }
}