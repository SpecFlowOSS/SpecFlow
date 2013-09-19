using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("gherkin")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class GherkinTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        [Import]
        IBiDiContainerProvider ContainerProvider = null;

        private class EditorCommandFilterInstance : IOleCommandTarget
        {
            private readonly EditorCommandFilter commandFilter;
            private readonly GherkinEditorContext editorContext;

            public EditorCommandFilterInstance(EditorCommandFilter commandFilter, GherkinEditorContext editorContext)
            {
                this.commandFilter = commandFilter;
                this.editorContext = editorContext;
            }

            public IOleCommandTarget Next { private get; set; }

            public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
            {
                if (commandFilter.QueryStatus(editorContext, pguidCmdGroup, prgCmds[0]))
                    return VSConstants.S_OK;

                return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }

            public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
            {
                int hresult = VSConstants.S_OK;
                if (!commandFilter.PreExec(editorContext, pguidCmdGroup, nCmdID))
                    hresult = Next.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                if (ErrorHandler.Succeeded(hresult))
                {
                    commandFilter.PostExec(editorContext, pguidCmdGroup, nCmdID, pvaIn);
                }

                return hresult;
            }
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);
            Debug.WriteLineIf(view != null, "No WPF editor view found");
            if (view == null)
                return;

            var languageService = GherkinLanguageServiceFactory.GetLanguageService(view.TextBuffer);
            var editorContext = new GherkinEditorContext(languageService, view);

            var editorCommandFilter = ContainerProvider.ObjectContainer.Resolve<EditorCommandFilter>();

            var commandFilter = new EditorCommandFilterInstance(editorCommandFilter, editorContext);

            IOleCommandTarget next;
            textViewAdapter.AddCommandFilter(commandFilter, out next);
            commandFilter.Next = next;
        }
    }
}