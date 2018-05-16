using System;
using System.CodeDom;
using System.Collections.Generic;
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
                    unitTestFeatureGenerator.TestclassNameFormat += $"_{featureGenerator.Key.UnitTestProvider}_{featureGenerator.Key.TargetFramework}_{featureGenerator.Key.ProjectFormat}_{featureGenerator.Key.ProgrammingLanguage}";
                }
            }
        }

        public CodeNamespace GenerateUnitTestFixture(SpecFlowDocument specFlowDocument, string testClassName, string targetNamespace)
        {
            CodeNamespace result = null;

            var specFlowFeature = specFlowDocument.SpecFlowFeature;
            if (specFlowFeature.HasTags())
            {
                if (specFlowFeature.Tags.Where(t => t.Name == "@SingleTestConfiguration").Any())
                {
                    return _defaultFeatureGenerator.GenerateUnitTestFixture(specFlowDocument, testClassName, targetNamespace);
                }
            }

            foreach (var featureGenerator in _featureGenerators)
            {
                if (specFlowFeature.HasTags())
                {
                    if (!IsForUnitTestProvider(featureGenerator, "XUnit") && HasFeatureTag(specFlowFeature, "@xunit"))
                    {
                        continue;
                    }

                    if (!IsForUnitTestProvider(featureGenerator, "MsTest") && HasFeatureTag(specFlowFeature, "@MsTest"))
                    {
                        continue;
                    }

                    if (!IsForUnitTestProvider(featureGenerator, "NUnit3") && HasFeatureTag(specFlowFeature, "@NUnit"))
                    {
                        continue;
                    }
                }

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

        private bool IsForUnitTestProvider(KeyValuePair<Combination, IFeatureGenerator> featureGenerator, string unitTestProvider)
        {
            return string.Compare(featureGenerator.Key.UnitTestProvider, unitTestProvider, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        private bool HasFeatureTag(SpecFlowFeature specFlowFeature, string tag)
        {
            return specFlowFeature.Tags.Where(t => string.Compare(t.Name, tag, StringComparison.CurrentCultureIgnoreCase) == 0).Any();
        }
    }
}