using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using BoDi;
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

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);
            Debug.WriteLineIf(view != null, "No WPF editor view found");
            if (view == null)
                return;

            var languageService = GherkinLanguageServiceFactory.GetLanguageService(view.TextBuffer);

            var commandFilter = new EditorCommandFilter(ContainerProvider.ObjectContainer, view, languageService);

            IOleCommandTarget next;
            textViewAdapter.AddCommandFilter(commandFilter, out next);
            commandFilter.Next = next;
        }
    }

    internal class EditorCommandFilter : IOleCommandTarget
    {
        private readonly EditorCommands editorCommands;

        public IOleCommandTarget Next { get; set; }

        public EditorCommandFilter(IObjectContainer container, IWpfTextView textView, GherkinLanguageService languageService)
        {
            editorCommands = new EditorCommands(container, languageService, textView);
        }

        private char GetTypeChar(IntPtr pvaIn)
        {
            return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            bool handled = false;
            int hresult = VSConstants.S_OK;

            // 1. Pre-process
            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                switch ((VSConstants.VSStd97CmdID)nCmdID)
                {
                    case VSConstants.VSStd97CmdID.GotoDefn:
                        handled = editorCommands.GoToDefinition();
                        break;
                }
            }
            if (pguidCmdGroup == GuidList.guidSpecFlowCmdSet)
            {
                switch ((SpecFlowCmdSet)nCmdID)
                {
                    case SpecFlowCmdSet.RunScenarios:
                        handled = editorCommands.RunScenarios();
                        break;
                    case SpecFlowCmdSet.DebugScenarios:
                        handled = editorCommands.DebugScenarios();
                        break;
                    case SpecFlowCmdSet.GoToStepDefinition:
                        handled = editorCommands.GoToDefinition();
                        break;
                }
            }

            if (!handled)
                hresult = Next.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (ErrorHandler.Succeeded(hresult))
            {
                if (pguidCmdGroup == VSConstants.VSStd2K)
                {
                    switch ((VSConstants.VSStd2KCmdID)nCmdID)
                    {
                        case VSConstants.VSStd2KCmdID.TYPECHAR:
                            var ch = GetTypeChar(pvaIn);
                            if (ch == '|')
                                editorCommands.FormatTable();
                            break;
                    }
                }
//TODO: uncomment this to add further command handlers
//                if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
//                {
//                    switch ((VSConstants.VSStd97CmdID)nCmdID)
//                    {
//                    }
//                }
            }

            return hresult;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                switch ((VSConstants.VSStd97CmdID)prgCmds[0].cmdID)
                {
                    case VSConstants.VSStd97CmdID.GotoDefn:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        if (editorCommands.CanGoToDefinition())
                            return VSConstants.S_OK;
                        break;
                }
            }
            if (pguidCmdGroup == GuidList.guidSpecFlowCmdSet)
            {
                switch ((SpecFlowCmdSet)prgCmds[0].cmdID)
                {
                    case SpecFlowCmdSet.RunScenarios:
                    case SpecFlowCmdSet.DebugScenarios:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                    case SpecFlowCmdSet.GoToStepDefinition:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        if (editorCommands.CanGoToDefinition())
                            return VSConstants.S_OK;
                        break;
                }
            }
            return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }

}