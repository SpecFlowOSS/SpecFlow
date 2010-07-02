using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace GherkinFileClassifier
{

    #region Provider definition
    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers. Since 
    /// the content type is set to "text", this classifier applies to all text files
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("gherkin")]
    internal class GherkinFileClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        [Import]
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => 
                new GherkinFileClassifier(buffer, ClassificationRegistry, BufferTagAggregatorFactoryService));
        }
    }
    #endregion //provider def

    #region Classifier
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    class GherkinFileClassifier : IClassifier
    {
        private readonly IClassificationTypeRegistryService classificationTypeRegistryService;

        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService { get; private set; }

        internal GherkinFileClassifier(ITextBuffer buffer, IClassificationTypeRegistryService classificationTypeRegistryService, IBufferTagAggregatorFactoryService bufferTagAggregatorFactoryService)
        {
            BufferTagAggregatorFactoryService = bufferTagAggregatorFactoryService;
            this.classificationTypeRegistryService = classificationTypeRegistryService;

            buffer.Changed += new EventHandler<TextContentChangedEventArgs>(buffer_Changed);
        }

        private IList<ClassificationSpan> lastClassification = null;

        void buffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            var chStart = e.Changes.Min(ch => ch.NewPosition);
            var chEnd = e.Changes.Max(ch => ch.NewPosition + ch.NewLength);
            var chSpan = new Span(chStart, chEnd);

            SnapshotSpan allText = new SnapshotSpan(e.After, 0, e.After.Length);
            var newClassification = SyntaxColorer.GetClassificationSpans(allText, classificationTypeRegistryService);

            bool fullRefresh = false;

            if (lastClassification != null)
            {
                var overlappingClassifications = lastClassification.Where(cs => cs.Span.OverlapsWith(chSpan)).Select(cs => cs.ClassificationType);
                fullRefresh |= overlappingClassifications.Any(cl => cl.Classification.Equals("string", StringComparison.InvariantCultureIgnoreCase));
            }
            if (!fullRefresh)
            {
                var overlappingClassifications = newClassification.Where(cs => cs.Span.OverlapsWith(chSpan)).Select(cs => cs.ClassificationType);
                fullRefresh |= overlappingClassifications.Any(cl => cl.Classification.Equals("string", StringComparison.InvariantCultureIgnoreCase));
            }

            lastClassification = newClassification;

            if (fullRefresh)
            {
                if (ClassificationChanged != null)
                    ClassificationChanged(this, new ClassificationChangedEventArgs(
                        new SnapshotSpan(e.After, 0, e.After.Length)));
            }
        }

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="span">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (lastClassification == null)
            {
                SnapshotSpan allText = new SnapshotSpan(span.Snapshot, 0, span.Snapshot.Length);
                lastClassification = SyntaxColorer.GetClassificationSpans(allText, classificationTypeRegistryService);
            }

            return lastClassification;
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the clssification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }
    #endregion //Classifier
}
