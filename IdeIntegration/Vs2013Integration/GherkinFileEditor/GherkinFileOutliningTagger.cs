using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Options;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    #region Provider definition
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("gherkin")]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        internal IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        [Import]
        internal IGherkinBufferServiceManager GherkinBufferServiceManager = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (!IntegrationOptionsProvider.GetOptions().EnableOutlining)
                return null;

            return (ITagger<T>)GherkinBufferServiceManager.GetOrCreate(buffer, () =>
                new GherkinFileOutliningTagger(GherkinLanguageServiceFactory.GetLanguageService(buffer)));
        }
    }
    #endregion

    internal class GherkinFileOutliningTagger : ITagger<IOutliningRegionTag>, IDisposable
    {
        private readonly GherkinLanguageService gherkinLanguageService;

        public GherkinFileOutliningTagger(GherkinLanguageService gherkinLanguageService)
        {
            this.gherkinLanguageService = gherkinLanguageService;

            gherkinLanguageService.FileScopeChanged += GherkinLanguageServiceOnFileScopeChanged;
        }

        private void GherkinLanguageServiceOnFileScopeChanged(object sender, GherkinFileScopeChange gherkinFileScopeChange)
        {
            if (TagsChanged != null)
            {
                SnapshotSpanEventArgs args = new SnapshotSpanEventArgs(gherkinFileScopeChange.CreateChangeSpan());
                TagsChanged(this, args);
            }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var fileScope = gherkinLanguageService.GetFileScope(waitForResult: false);
            if (fileScope == null)
                return new ITagSpan<IOutliningRegionTag>[0];
            return fileScope.GetTags(spans);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Dispose()
        {
            gherkinLanguageService.FileScopeChanged -= GherkinLanguageServiceOnFileScopeChanged;
        }
    }
}
