using System.IO;
using SpecFlow.ExternalData.SpecFlowPlugin.Transformation;
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
        private readonly IncludeExternalDataTransformation _includeExternalDataTransformation;

        public ExternalDataTestGenerator(SpecFlowConfiguration specFlowConfiguration, ProjectSettings projectSettings, ITestHeaderWriter testHeaderWriter, ITestUpToDateChecker testUpToDateChecker, IFeatureGeneratorRegistry featureGeneratorRegistry, CodeDomHelper codeDomHelper, IGherkinParserFactory gherkinParserFactory, IExternalDataFeaturePatcher externalDataFeaturePatcher, IncludeExternalDataTransformation includeExternalDataTransformation) 
            : base(specFlowConfiguration, projectSettings, testHeaderWriter, testUpToDateChecker, featureGeneratorRegistry, codeDomHelper, gherkinParserFactory)
        {
            _externalDataFeaturePatcher = externalDataFeaturePatcher;
            _includeExternalDataTransformation = includeExternalDataTransformation;
        }

        protected override SpecFlowDocument ParseContent(IGherkinParser parser, TextReader contentReader,
            SpecFlowDocumentLocation documentLocation)
        {
            var document = base.ParseContent(parser, contentReader, documentLocation);
            document = _includeExternalDataTransformation.TransformDocument(document);
            return _externalDataFeaturePatcher.PatchDocument(document); //TODO: remove old code
        }
    }
}
