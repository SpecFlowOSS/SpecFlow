using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    #region Provider definition
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("gherkin")]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            GherkinFileEditorParser parser =
                GherkinFileEditorParser.GetParser(buffer, ClassificationRegistry);

            return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() =>
                new GherkinFileOutliningTagger(parser));
        }
    }
    #endregion

    internal class GherkinFileOutliningTagger : ITagger<IOutliningRegionTag>
    {
        private readonly GherkinFileEditorParser parser;

        public GherkinFileOutliningTagger(GherkinFileEditorParser parser)
        {
            this.parser = parser;

            parser.TagsChanged += (sender, args) =>
            {
                if (TagsChanged != null)
                    TagsChanged(this, args);
            };
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return parser.GetTags(spans);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
