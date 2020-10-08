using Gherkin.Ast;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Generation;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    internal class MultiFeatureGenerator : IFeatureGenerator
    {
        private readonly IFeatureGenerator _defaultFeatureGenerator;
        private readonly KeyValuePair<Combination, IFeatureGenerator>[] _featureGenerators;
        private readonly List<string> _unitTestProviderTags = new List<string> { "xunit", "mstest", "nunit3" };

        public MultiFeatureGenerator(IEnumerable<KeyValuePair<Combination, IFeatureGenerator>> featureGenerators, IFeatureGenerator defaultFeatureGenerator)
        {
            _defaultFeatureGenerator = defaultFeatureGenerator;
            _featureGenerators = featureGenerators.ToArray();

            foreach (var featureGenerator in _featureGenerators)
            {
                if (featureGenerator.Value is UnitTestFeatureGenerator unitTestFeatureGenerator)
                {
                    unitTestFeatureGenerator.TestClassNameFormat += $"_{featureGenerator.Key.UnitTestProvider}_{featureGenerator.Key.TargetFramework}_{featureGenerator.Key.ProjectFormat}_{featureGenerator.Key.ProgrammingLanguage}";
                }
            }
        }

        public CodeNamespace GenerateUnitTestFixture(SpecFlowDocument specFlowDocument, string testClassName, string targetNamespace)
        {
            CodeNamespace result = null;
            bool onlyFullframework = false;

            var specFlowFeature = specFlowDocument.SpecFlowFeature;
            bool onlyDotNetCore = false;
            if (specFlowFeature.HasTags())
            {
                if (specFlowFeature.Tags.Where(t => t.Name == "@SingleTestConfiguration").Any())
                {
                    return _defaultFeatureGenerator.GenerateUnitTestFixture(specFlowDocument, testClassName, targetNamespace);
                }

                onlyFullframework = HasFeatureTag(specFlowFeature, "@fullframework");
                onlyDotNetCore = HasFeatureTag(specFlowFeature, "@dotnetcore");
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                onlyFullframework = false;
                onlyDotNetCore = true;
            }

            var tagsOfFeature = specFlowFeature.Tags.Select(t => t.Name);
            var unitTestProviders = tagsOfFeature.Where(t => _unitTestProviderTags.Where(utpt => string.Compare(t, "@" + utpt, StringComparison.CurrentCultureIgnoreCase) == 0).Any());

            foreach (var featureGenerator in GetFilteredFeatureGenerator(unitTestProviders, onlyFullframework, onlyDotNetCore))
            {
                var clonedDocument = CloneDocumentAndAddTag(specFlowDocument, featureGenerator.Key.UnitTestProvider);


                var featureGeneratorResult = featureGenerator.Value.GenerateUnitTestFixture(clonedDocument, testClassName, targetNamespace);

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

        private SpecFlowDocument CloneDocumentAndAddTag(SpecFlowDocument specFlowDocument, string unitTestProvider)
        {
            if (HasFeatureTag(specFlowDocument.SpecFlowFeature, unitTestProvider))
            {
                return specFlowDocument;
            }


            var tags = new List<Tag>();
            var specFlowFeature = specFlowDocument.SpecFlowFeature;
            tags.AddRange(specFlowFeature.Tags);
            tags.Add(new Tag(null, unitTestProvider));
            var feature = new SpecFlowFeature(tags.ToArray(),
                                              specFlowFeature.Location,
                                              specFlowFeature.Language,
                                              specFlowFeature.Keyword,
                                              specFlowFeature.Name,
                                              specFlowFeature.Description,
                                              specFlowFeature.Children.ToArray());

            return new SpecFlowDocument(feature, specFlowDocument.Comments.ToArray(), specFlowDocument.DocumentLocation);
        }

        private IEnumerable<KeyValuePair<Combination, IFeatureGenerator>> GetFilteredFeatureGenerator(IEnumerable<string> unitTestProviders, bool onlyFullframework, bool onlyDotNetCore)
        {
            if (!unitTestProviders.Any())
            {
                foreach (var featureGenerator in _featureGenerators)
                {
                    if (onlyFullframework)
                    {
                        if (featureGenerator.Key.TargetFramework == TestRunCombinations.TfmEnumValueNet461)
                        {
                            yield return featureGenerator;
                        }
                    }
                    else
                    {
                        if (onlyDotNetCore)
                        {
                            if (ShouldCompileForNetCore21(featureGenerator.Key)
                                || ShouldCompileForNetCore31(featureGenerator.Key) || ShouldCompileForNet50(featureGenerator.Key))
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

            foreach (string unitTestProvider in unitTestProviders)
            {
                foreach (var featureGenerator in _featureGenerators)
                {
                    if (IsForUnitTestProvider(featureGenerator, unitTestProvider))
                    {
                        if (onlyFullframework)
                        {
                            if (featureGenerator.Key.TargetFramework == TestRunCombinations.TfmEnumValueNet461)
                            {
                                yield return featureGenerator;
                            }
                        }
                        else
                        {
                            if (onlyDotNetCore)
                            {
                                if (ShouldCompileForNetCore21(featureGenerator.Key)
                                    || ShouldCompileForNetCore31(featureGenerator.Key) || ShouldCompileForNet50(featureGenerator.Key))
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
        }
        public bool ShouldCompileForNetCore21(Combination combination)
        {
            return combination.TargetFramework == TestRunCombinations.TfmEnumValueNetCore21
                   && RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public bool ShouldCompileForNetCore31(Combination combination)
        {
            return combination.TargetFramework == TestRunCombinations.TfmEnumValueNetCore31;
        }

        public bool ShouldCompileForNet50(Combination combination)
        {
            return combination.TargetFramework == TestRunCombinations.TfmEnumValueNet50;
        }

        private bool IsForUnitTestProvider(KeyValuePair<Combination, IFeatureGenerator> featureGenerator, string unitTestProvider)
        {
            return string.Compare("@" + featureGenerator.Key.UnitTestProvider, unitTestProvider, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        private bool HasFeatureTag(SpecFlowFeature specFlowFeature, string tag)
        {
            return specFlowFeature.Tags.Any(t => string.Compare(t.Name, tag, StringComparison.CurrentCultureIgnoreCase) == 0);
        }
    }
}
