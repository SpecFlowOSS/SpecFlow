using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    internal class MultiFeatureGenerator : IFeatureGenerator
    {
        private readonly IFeatureGenerator _defaultFeatureGenerator;
        private readonly KeyValuePair<Combination, IFeatureGenerator>[] _featureGenerators;

        public MultiFeatureGenerator(IEnumerable<KeyValuePair<Combination, IFeatureGenerator>> featureGenerators, IFeatureGenerator defaultFeatureGenerator)
        {
            _defaultFeatureGenerator = defaultFeatureGenerator;
            _featureGenerators = featureGenerators.ToArray();

            foreach (var featureGenerator in _featureGenerators)
            {
                if (featureGenerator.Value is UnitTestFeatureGenerator unitTestFeatureGenerator)
                {
                    unitTestFeatureGenerator.TestclassNameFormat += $"_{featureGenerator.Key.ProgrammingLanguage}_{featureGenerator.Key.ProjectFormat}_{featureGenerator.Key.TargetFramework}";
                }
            }
        }

        public CodeNamespace GenerateUnitTestFixture(SpecFlowDocument specFlowDocument, string testClassName, string targetNamespace)
        {
            CodeNamespace result = null;

            if (specFlowDocument.SpecFlowFeature.HasTags())
            {
                if (specFlowDocument.SpecFlowFeature.Tags.Where(t => t.Name == "@SingleTestConfiguration").Any())
                {
                    return _defaultFeatureGenerator.GenerateUnitTestFixture(specFlowDocument, testClassName, targetNamespace);
                }
            }

            foreach (var featureGenerator in _featureGenerators)
            {
                var featureGeneratorResult = featureGenerator.Value.GenerateUnitTestFixture(specFlowDocument, testClassName, targetNamespace);

                if (result == null)
                {
                    result = featureGeneratorResult;
                }
                else
                {
                    foreach (CodeTypeDeclaration type in featureGeneratorResult.Types)
                    {
                        result.Types.Add(type);
                    }
                }
            }

            return result;
        }
    }
}