using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    #region Provider definition
    [Export(typeof(IClassifierProvider))]
    [ContentType("gherkin")]
    internal class GherkinFileClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            GherkinFileEditorParser parser = 
                GherkinFileEditorParser.GetParser(buffer, ClassificationRegistry);

            return buffer.Properties.GetOrCreateSingletonProperty(() => 
                new GherkinFileClassifier(parser));
        }
    }
    #endregion //provider def

    internal class GherkinFileClassifier : IClassifier
    {
        private readonly GherkinFileEditorParser parser;

        public GherkinFileClassifier(GherkinFileEditorParser parser)
        {
            this.parser = parser;

            parser.ClassificationChanged += (sender, args) =>
            {
                if (ClassificationChanged != null)
                    ClassificationChanged(this, args);
            };
        }

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="span">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return parser.GetClassificationSpans(span);
        }

        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the clssification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}
