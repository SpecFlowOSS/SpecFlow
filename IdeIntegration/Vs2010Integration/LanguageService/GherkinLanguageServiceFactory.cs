using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinLanguageServiceFactory
    {
        GherkinLanguageService GetLanguageService(ITextBuffer textBuffer);
    }

//    [Export(typeof(IWpfTextViewConnectionListener))]
//    [ContentType("gherkin")]
//    [TextViewRole(PredefinedTextViewRoles.Interactive)]
//    public class Bla : IWpfTextViewConnectionListener
//    {
//        [Import]
//        internal IVisualStudioTracer VisualStudioTracer = null;
//
//        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
//        {
//            VisualStudioTracer.Trace("SubjectBuffersConnected", "Bla");
//        }
//
//        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
//        {
//            VisualStudioTracer.Trace("SubjectBuffersDisconnected", "Bla");
//        }
//    }

    [Export(typeof(IGherkinLanguageServiceFactory))]
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType("gherkin")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class GherkinLanguageServiceFactory : IGherkinLanguageServiceFactory, IWpfTextViewConnectionListener
    {
        private const string LS_KEY = "GLS";

        [Import]
        internal IProjectScopeFactory ProjectScopeFactory = null;

        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        [Import]
        internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        internal IVisualStudioTracer VisualStudioTracer = null;

        public GherkinLanguageService GetLanguageService(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(LS_KEY, () => CreateLanguageService(textBuffer));
        }

        private GherkinLanguageService CreateLanguageService(ITextBuffer textBuffer)
        {
            var project = VsxHelper.GetCurrentProject(textBuffer, AdaptersFactory, ServiceProvider);
            var projectScope = ProjectScopeFactory.GetProjectScope(project);
            var languageService = new GherkinLanguageService(projectScope, VisualStudioTracer);

            textBuffer.Changed +=
                (sender, args) => languageService.TextBufferChanged(GetTextBufferChange(args));

            languageService.Initialize(textBuffer);

            return languageService;
        }

        private GherkinTextBufferChange GetTextBufferChange(TextContentChangedEventArgs textContentChangedEventArgs)
        {
            Debug.Assert(textContentChangedEventArgs.Changes.Count > 0);

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

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            //nop
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            VisualStudioTracer.Trace("SubjectBuffersDisconnected", "GherkinLanguageServiceFactory");
            
            foreach (var subjectBuffer in subjectBuffers)
            {
                GherkinLanguageService languageService;
                if (subjectBuffer.Properties.TryGetProperty<GherkinLanguageService>(LS_KEY, out languageService))
                {
                    subjectBuffer.Properties.RemoveProperty(LS_KEY);
                    languageService.Dispose();
                }
            }
        }
    }
}