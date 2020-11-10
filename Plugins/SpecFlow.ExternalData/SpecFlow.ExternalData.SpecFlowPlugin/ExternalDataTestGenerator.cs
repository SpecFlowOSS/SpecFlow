using System.IO;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public class ExternalDataTestGenerator : TestGenerator
    {
        private readonly IExternalDataFeaturePatcher _externalDataFeaturePatcher;
        public ExternalDataTestGenerator(SpecFlowConfiguration specFlowConfiguration, ProjectSettings projectSettings, ITestHeaderWriter testHeaderWriter, ITestUpToDateChecker testUpToDateChecker, IFeatureGeneratorRegistry featureGeneratorRegistry, CodeDomHelper codeDomHelper, IGherkinParserFactory gherkinParserFactory, IExternalDataFeaturePatcher externalDataFeaturePatcher) : base(specFlowConfiguration, projectSettings, testHeaderWriter, testUpToDateChecker, featureGeneratorRegistry, codeDomHelper, gherkinParserFactory)
        {
            _externalDataFeaturePatcher = externalDataFeaturePatcher;
        }

        protected override SpecFlowDocument ParseContent(IGherkinParser parser, TextReader contentReader,
            SpecFlowDocumentLocation documentLocation)
        {
            var document = base.ParseContent(parser, contentReader, documentLocation);
            return _externalDataFeaturePatcher.PatchDocument(document);
        }
    }
}
