using System;
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
                    unitTestFeatureGenerator.TestclassNameFormat += $"_{featureGenerator.Key.UnitTestProvider}_{featureGenerator.Key.TargetFramework}_{featureGenerator.Key.ProjectFormat}_{featureGenerator.Key.ProgrammingLanguage}";
                }
            }
        }

        private List<string> _unitTestProviderTags = new List<string>() { "xunit", "mstest", "nunit3" };

        public CodeNamespace GenerateUnitTestFixture(SpecFlowDocument specFlowDocument, string testClassName, string targetNamespace)
        {
            CodeNamespace result = null;
            bool onlyFullframework = false;

            //Debugger.Launch();

            var specFlowFeature = specFlowDocument.SpecFlowFeature;
            if (specFlowFeature.HasTags())
            {
                if (specFlowFeature.Tags.Where(t => t.Name == "@SingleTestConfiguration").Any())
                {
                    return _defaultFeatureGenerator.GenerateUnitTestFixture(specFlowDocument, testClassName, targetNamespace);
                }

                onlyFullframework = HasFeatureTag(specFlowFeature, "@fullframework");
            }


            var tagsOfFeature = specFlowFeature.Tags.Select(t => t.Name);
            var unitTestProviders = tagsOfFeature.Where(t => _unitTestProviderTags.Where(utpt => string.Compare(t, "@"+utpt, StringComparison.CurrentCultureIgnoreCase) == 0).Any());

            foreach (var featureGenerator in GetFilteredFeatureGenerator(unitTestProviders, onlyFullframework))
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

            if (result == null)
            {
                result = new CodeNamespace(targetNamespace);
            }

            return result;
        }

        private IEnumerable<KeyValuePair<Combination, IFeatureGenerator>> GetFilteredFeatureGenerator(IEnumerable<string> unitTestProviders, bool onlyFullframework)
        {
            if (!unitTestProviders.Any())
            {
                foreach (var featureGenerator in _featureGenerators)
                {
                    yield return featureGenerator;
                }
            }

            foreach (string unitTestProvider in unitTestProviders)
            {
                foreach (var featureGenerator in _featureGenerators)
                {
                    if (IsForUnitTestProvider(featureGenerator, unitTestProvider))
                    {
                        if (onlyFullframework)
                        {
                            if (featureGenerator.Key.TargetFramework == TestRunCombinations.TFM_FullFramework)
                            {
                                yield return featureGenerator;
                            }
                        }
                        else
                        {
                            yield return featureGenerator;
                        }
                    }
                }
            }
        }

        private bool IsForUnitTestProvider(KeyValuePair<Combination, IFeatureGenerator> featureGenerator, string unitTestProvider)
        {
            return string.Compare("@"+featureGenerator.Key.UnitTestProvider, unitTestProvider, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        private bool HasFeatureTag(SpecFlowFeature specFlowFeature, string tag)
        {
            return specFlowFeature.Tags.Any(t => string.Compare(t.Name, tag, StringComparison.CurrentCultureIgnoreCase) == 0);
        }
    }
}