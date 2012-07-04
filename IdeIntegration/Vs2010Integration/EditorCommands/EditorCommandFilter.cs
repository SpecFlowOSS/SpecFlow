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
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
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
        private readonly IIdeTracer tracer;
        private readonly EditorCommands editorCommands;

        public IOleCommandTarget Next { get; set; }

        public EditorCommandFilter(IObjectContainer container, IWpfTextView textView, GherkinLanguageService languageService)
        {
            editorCommands = new EditorCommands(container, languageService, textView);
            tracer = languageService.ProjectScope.Tracer;
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
                var vsStd97CmdId = (VSConstants.VSStd97CmdID)nCmdID;
#if TRACE_VS_COMMANDS
                if (vsStd97CmdId != VSConstants.VSStd97CmdID.SearchCombo && vsStd97CmdId != VSConstants.VSStd97CmdID.SolutionCfg)
                    tracer.Trace("Exec/VSStd97CmdID:{0}", this, vsStd97CmdId);
#endif
                switch (vsStd97CmdId)
                {
                    case VSConstants.VSStd97CmdID.GotoDefn:
                        handled = editorCommands.GoToDefinition();
                        break;
                }
            }
            else if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                var vsStd2KCmdId = (VSConstants.VSStd2KCmdID)nCmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/VSStd2KCmdID:{0}", this, vsStd2KCmdId);
#endif
                switch (vsStd2KCmdId)
                {
                    case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                        handled = editorCommands.CommentOrUncommentSelection(CommentUncommentAction.Comment);
                        break;
                    case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                        handled = editorCommands.CommentOrUncommentSelection(CommentUncommentAction.Uncomment);
                        break;
                }
            }
            else if (pguidCmdGroup == GuidList.guidSpecFlowCmdSet)
            {
                var specFlowCmdSet = (SpecFlowCmdSet)nCmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/SpecFlowCmdSet:{0}", this, specFlowCmdSet);
#endif
                switch (specFlowCmdSet)
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
            else if(pguidCmdGroup == ReSharperCommandGroups.CommandGroup)
            {
                var reSharperCmd = (ReSharperCommand)nCmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/ReSharperCommand:{0}", this, reSharperCmd);
#endif
                switch (reSharperCmd)
                {
                    case ReSharperCommand.GotoDeclaration:
                        handled = editorCommands.GoToDefinition();
                        break;
                    case ReSharperCommand.LineComment:
                        handled = editorCommands.CommentOrUncommentSelection(CommentUncommentAction.Toggle);
                        break;
                    case ReSharperCommand.UnitTestRunContext:
                        handled = editorCommands.RunScenarios(TestRunnerTool.ReSharper);
                        break;
                    case ReSharperCommand.UnitTestDebugContext:
                        handled = editorCommands.DebugScenarios(TestRunnerTool.ReSharper);
                        break;
                }
            }
            else
            {
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/Other:{0} / {1}", this, pguidCmdGroup, nCmdID);
#endif
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
                var vsStd97CmdId = (VSConstants.VSStd97CmdID)prgCmds[0].cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/VSStd97CmdID:{0}", this, vsStd97CmdId);
#endif
                switch (vsStd97CmdId)
                {
                    case VSConstants.VSStd97CmdID.GotoDefn:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        if (editorCommands.CanGoToDefinition())
                            return VSConstants.S_OK;
                        break;
                }
            }
            else if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                var vsStd2KCmdId = (VSConstants.VSStd2KCmdID)prgCmds[0].cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/VSStd2KCmdID:{0}", this, vsStd2KCmdId);
#endif
                switch (vsStd2KCmdId)
                {
                    case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }
            else if (pguidCmdGroup == GuidList.guidSpecFlowCmdSet)
            {
                var specFlowCmdSet = (SpecFlowCmdSet)prgCmds[0].cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/SpecFlowCmdSet:{0}", this, specFlowCmdSet);
#endif
                switch (specFlowCmdSet)
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
            else if(pguidCmdGroup == ReSharperCommandGroups.CommandGroup)
            {
                var reSharperCmd = (ReSharperCommand)prgCmds[0].cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/ReSharperCommand:{0}", this, reSharperCmd);
#endif
                switch (reSharperCmd)
                {
                    case ReSharperCommand.GotoDeclaration:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        if (editorCommands.CanGoToDefinition())
                            return VSConstants.S_OK;
                        break;
                    case ReSharperCommand.LineComment:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                    case ReSharperCommand.UnitTestRunContext:
                    case ReSharperCommand.UnitTestDebugContext:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }
            else
            {
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/Other:{0} / {1}", this, pguidCmdGroup, prgCmds[0].cmdID);
#endif
            }
            return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }

}