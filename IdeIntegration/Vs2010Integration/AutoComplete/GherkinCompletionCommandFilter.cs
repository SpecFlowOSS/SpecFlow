using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("gherkin")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class GherkinTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        ICompletionBroker CompletionBroker = null;

        [Import]
        IGherkinProcessorServices GherkinProcessorServices = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (!GherkinProcessorServices.GetOptions().EnableIntelliSense)
                return;

            IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);
            Debug.WriteLineIf(view != null, "No WPF editor view found");
            if (view == null)
                return;

            var commandFilter = new GherkinCompletionCommandFilter(view, CompletionBroker, GherkinProcessorServices);

            IOleCommandTarget next;
            textViewAdapter.AddCommandFilter(commandFilter, out next);
            commandFilter.Next = next;
        }
    }

    internal class GherkinCompletionCommandFilter : CompletionCommandFilter
    {
        public GherkinCompletionCommandFilter(IWpfTextView textView, ICompletionBroker broker, IGherkinProcessorServices gherkinProcessorServices) : base(textView, broker)
        {
            GherkinProcessorServices = gherkinProcessorServices;
        }

        public IGherkinProcessorServices GherkinProcessorServices { get; private set; }
        
        /// <summary>
        /// Displays completion after typing a space after a step keyword
        /// </summary>
        protected override bool ShouldCompletionBeDiplayed(SnapshotPoint caret)
        {
            var lineStart = caret.GetContainingLine().Start.Position;
            var lineBeforeCaret = caret.Snapshot.GetText(lineStart, caret.Position - lineStart);

            if (lineBeforeCaret.Length > 0 &&
                char.IsWhiteSpace(lineBeforeCaret[lineBeforeCaret.Length - 1]))
            {
                string keyword = lineBeforeCaret.Substring(0, lineBeforeCaret.Length - 1).TrimStart();
                var gherkinDialect = GherkinProcessorServices.GetGherkinDialect(caret.Snapshot.TextBuffer);
                return gherkinDialect.IsStepKeyword(keyword);
            }

            return false;
        }
    }
}