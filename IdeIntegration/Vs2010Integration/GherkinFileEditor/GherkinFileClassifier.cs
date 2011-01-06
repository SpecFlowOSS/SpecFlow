using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE80;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{

    #region Provider definition
    [Export(typeof(IClassifierProvider))]
    [ContentType("gherkin")]
    internal class GherkinFileClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        [Import]
        internal IGherkinProcessorServices GherkinProcessorServices = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (!GherkinProcessorServices.GetOptions().EnableSyntaxColoring)
                return null;

            return buffer.Properties.GetOrCreateSingletonProperty(() => 
                new GherkinFileClassifier(GherkinLanguageServiceFactory.GetLanguageService(buffer)));
        }

    }
    #endregion //provider def

    internal class GherkinFileClassifier : IClassifier
    {
        private readonly GherkinLanguageService gherkinLanguageService;

        public GherkinFileClassifier(GherkinLanguageService gherkinLanguageService)
        {
            this.gherkinLanguageService = gherkinLanguageService;

            gherkinLanguageService.FileScopeChanged += GherkinLanguageServiceOnFileScopeChanged;
        }

        private void GherkinLanguageServiceOnFileScopeChanged(object sender, GherkinFileScopeChange gherkinFileScopeChange)
        {
            if (ClassificationChanged != null)
            {
                ClassificationChangedEventArgs args = new ClassificationChangedEventArgs(gherkinFileScopeChange.CreateChangeSpan());
                ClassificationChanged(this, args);
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
            var fileScope = gherkinLanguageService.GetFileScope();
            return fileScope.GetClassificationSpans(span);
        }

        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the clssification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}
