using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Gherkin.Ast;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Generator.Generation
{
    public class UnitTestMethodGenerator
    {
        private const string IGNORE_TAG = "@Ignore";
        private const string TESTRUNNER_FIELD = "testRunner";
        private readonly CodeDomHelper _codeDomHelper;
        private readonly IDecoratorRegistry _decoratorRegistry;
        private readonly ScenarioPartHelper _scenarioPartHelper;
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly IUnitTestGeneratorProvider _unitTestGeneratorProvider;

        public UnitTestMethodGenerator(IUnitTestGeneratorProvider unitTestGeneratorProvider, IDecoratorRegistry decoratorRegistry, CodeDomHelper codeDomHelper, ScenarioPartHelper scenarioPartHelper, SpecFlowConfiguration specFlowConfiguration)
        {
            _unitTestGeneratorProvider = unitTestGeneratorProvider;
            _decoratorRegistry = decoratorRegistry;
            _codeDomHelper = codeDomHelper;
            _scenarioPartHelper = scenarioPartHelper;
            _specFlowConfiguration = specFlowConfiguration;
        }

        private IEnumerable<ScenarioDefinitionInFeatureFile> GetScenarioDefinitions(SpecFlowFeature feature)
        {
            IEnumerable<ScenarioDefinitionInFeatureFile> GetScenarioDefinitionsOfRule(IEnumerable<IHasLocation> items, Rule rule)
                => items.OfType<StepsContainer>()
                        .Where(child => child is not Background)
                        .Select(sd => new ScenarioDefinitionInFeatureFile(sd, feature, rule));

            return 
                GetScenarioDefinitionsOfRule(feature.Children, null)
                    .Concat(feature.Children.OfType<Rule>().SelectMany(rule => GetScenarioDefinitionsOfRule(rule.Children, rule)));
        }

        public void CreateUnitTests(SpecFlowFeature feature, TestClassGenerationContext generationContext)
        {
            foreach (var scenarioDefinition in GetScenarioDefinitions(feature))
            {
                CreateUnitTest(generationContext, scenarioDefinition);
            }
        }

        private void CreateUnitTest(TestClassGenerationContext generationContext, ScenarioDefinitionInFeatureFile scenarioDefinitionInFeatureFile)
        {
            if (string.IsNullOrEmpty(scenarioDefinitionInFeatureFile.ScenarioDefinition.Name))
            {
                throw new TestGeneratorException("The scenario must have a title specified.");
            }

            if (scenarioDefinitionInFeatureFile.IsScenarioOutline)
            {
                GenerateScenarioOutlineTest(generationContext, scenarioDefinitionInFeatureFile);
            }
            else
            {
                GenerateTest(generationContext, scenarioDefinitionInFeatureFile);
            }
        }

        private void GenerateScenarioOutlineTest(TestClassGenerationContext generationContext, ScenarioDefinitionInFeatureFile scenarioDefinitionInFeatureFile)
        {
            var scenarioOutline = scenarioDefinitionInFeatureFile.ScenarioOutline;
            ValidateExampleSetConsistency(scenarioOutline);

            var paramToIdentifier = CreateParamToIdentifierMapping(scenarioOutline);

            var scenarioOutlineTestMethod = CreateScenarioOutlineTestMethod(generationContext, scenarioOutline, paramToIdentifier);
            var exampleTagsParam = new CodeVariableReferenceExpression(GeneratorConstants.SCENARIO_OUTLINE_EXAMPLE_TAGS_PARAMETER);
            if (generationContext.GenerateRowTests)
            {
                GenerateScenarioOutlineExamplesAsRowTests(generationContext, scenarioOutline, scenarioOutlineTestMethod);
            }
            else
            {
                GenerateScenarioOutlineExamplesAsIndividualMethods(scenarioOutline, generationContext, scenarioOutlineTestMethod, paramToIdentifier);
            }

            GenerateTestBody(generationContext, scenarioDefinitionInFeatureFile, scenarioOutlineTestMethod, exampleTagsParam, paramToIdentifier);
        }

        private void GenerateTest(TestClassGenerationContext generationContext, ScenarioDefinitionInFeatureFile scenarioDefinitionInFeatureFile)
        {
            var testMethod = CreateTestMethod(generationContext, scenarioDefinitionInFeatureFile.ScenarioDefinition, null);
            GenerateTestBody(generationContext, scenarioDefinitionInFeatureFile, testMethod);
        }

        private void ValidateExampleSetConsistency(ScenarioOutline scenarioOutline)
        {
            if (scenarioOutline.Examples.Count() <= 1)
            {
                return;
            }

            var firstExamplesHeader = scenarioOutline.Examples.First().TableHeader.Cells.Select(c => c.Value).ToArray();

            //check params
            if (scenarioOutline.Examples
                .Skip(1)
                .Select(examples => examples.TableHeader.Cells.Select(c => c.Value))
                .Any(paramNames => !paramNames.SequenceEqual(firstExamplesHeader)))
            {
                throw new TestGeneratorException("The example sets must provide the same parameters.");
            }
        }

        private IEnumerable<string> GetNonIgnoreTags(IEnumerable<Tag> tags)
        {
            return tags.Where(t => !t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t.GetNameWithoutAt());
        }

        private bool HasIgnoreTag(IEnumerable<Tag> tags)
        {
            return tags.Any(t => t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase));
        }

        private void GenerateTestBody(
            TestClassGenerationContext generationContext,
            ScenarioDefinitionInFeatureFile scenarioDefinitionInFeatureFile,
            CodeMemberMethod testMethod,
            CodeExpression additionalTagsExpression = null, ParameterSubstitution paramToIdentifier = null)
        {
            var scenarioDefinition = scenarioDefinitionInFeatureFile.ScenarioDefinition;
            var feature = scenarioDefinitionInFeatureFile.Feature;

            //call test setup
            //ScenarioInfo scenarioInfo = new ScenarioInfo("xxxx", tags...);
            CodeExpression inheritedTagsExpression;
            var featureTagsExpression = new CodeFieldReferenceExpression(null, GeneratorConstants.FEATURE_TAGS_VARIABLE_NAME);
            if (scenarioDefinitionInFeatureFile.Rule != null && scenarioDefinitionInFeatureFile.Rule.Tags.Any())
            {
                var tagHelperReference = new CodeTypeReferenceExpression(nameof(TagHelper));
                var ruleTagsExpression = _scenarioPartHelper.GetStringArrayExpression(scenarioDefinitionInFeatureFile.Rule.Tags);
                inheritedTagsExpression = new CodeMethodInvokeExpression(tagHelperReference, nameof(TagHelper.CombineTags), featureTagsExpression, ruleTagsExpression);
            }
            else
            {
                inheritedTagsExpression = featureTagsExpression;
            }

            CodeExpression tagsExpression;
            if (additionalTagsExpression == null)
            {
                tagsExpression = _scenarioPartHelper.GetStringArrayExpression(scenarioDefinition.GetTags());
            }
            else if (!scenarioDefinition.HasTags())
            {
                tagsExpression = additionalTagsExpression;
            }
            else
            {
                // merge tags list
                // var tags = tags1
                // if (tags2 != null)
                //   tags = Enumerable.ToArray(Enumerable.Concat(tags1, tags1));
                testMethod.Statements.Add(
                    new CodeVariableDeclarationStatement(typeof(string[]), "__tags", _scenarioPartHelper.GetStringArrayExpression(scenarioDefinition.GetTags())));
                tagsExpression = new CodeVariableReferenceExpression("__tags");
                testMethod.Statements.Add(
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            additionalTagsExpression,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeAssignStatement(
                            tagsExpression,
                            new CodeMethodInvokeExpression(
                                new CodeTypeReferenceExpression(typeof(Enumerable)),
                                "ToArray",
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(typeof(Enumerable)),
                                    "Concat",
                                    tagsExpression,
                                    additionalTagsExpression)))));
            }


            AddVariableForTags(testMethod, tagsExpression);

            AddVariableForArguments(testMethod, paramToIdentifier);

            testMethod.Statements.Add(
                new CodeVariableDeclarationStatement(typeof(ScenarioInfo), "scenarioInfo",
                    new CodeObjectCreateExpression(typeof(ScenarioInfo),
                        new CodePrimitiveExpression(scenarioDefinition.Name),
                        new CodePrimitiveExpression(scenarioDefinition.Description),
                        new CodeVariableReferenceExpression(GeneratorConstants.SCENARIO_TAGS_VARIABLE_NAME),
                        new CodeVariableReferenceExpression(GeneratorConstants.SCENARIO_ARGUMENTS_VARIABLE_NAME),
                        inheritedTagsExpression)));

            GenerateScenarioInitializeCall(generationContext, scenarioDefinition, testMethod);

            GenerateTestMethodBody(generationContext, scenarioDefinitionInFeatureFile, testMethod, paramToIdentifier, feature);

            GenerateScenarioCleanupMethodCall(generationContext, testMethod);
        }

        private void AddVariableForTags(CodeMemberMethod testMethod, CodeExpression tagsExpression)
        {
            var tagVariable = new CodeVariableDeclarationStatement(typeof(string[]), GeneratorConstants.SCENARIO_TAGS_VARIABLE_NAME, tagsExpression);

            testMethod.Statements.Add(tagVariable);
        }

        private void AddVariableForArguments(CodeMemberMethod testMethod, ParameterSubstitution paramToIdentifier)
        {
            var argumentsExpression = new CodeVariableDeclarationStatement(
                typeof(OrderedDictionary),
                GeneratorConstants.SCENARIO_ARGUMENTS_VARIABLE_NAME,
                new CodeObjectCreateExpression(typeof(OrderedDictionary)));

            testMethod.Statements.Add(argumentsExpression);

            if (paramToIdentifier != null)
            {
                foreach (var parameter in paramToIdentifier)
                {
                    var addArgumentExpression = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodeTypeReferenceExpression(new CodeTypeReference(GeneratorConstants.SCENARIO_ARGUMENTS_VARIABLE_NAME)),
                            nameof(OrderedDictionary.Add)),
                        new CodePrimitiveExpression(parameter.Key),
                        new CodeVariableReferenceExpression(parameter.Value));

                    testMethod.Statements.Add(addArgumentExpression);
                }
            }
        }

        internal void GenerateTestMethodBody(TestClassGenerationContext generationContext, ScenarioDefinitionInFeatureFile scenarioDefinition, CodeMemberMethod testMethod, ParameterSubstitution paramToIdentifier, SpecFlowFeature feature)
        {
            var scenario = scenarioDefinition.Scenario;

            var statementsWhenScenarioIsIgnored = new CodeStatement[] { new CodeExpressionStatement(CreateTestRunnerSkipScenarioCall()) };

            var callScenarioStartMethodExpression = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), generationContext.ScenarioStartMethod.Name);

            _codeDomHelper.MarkCodeMethodInvokeExpressionAsAwait(callScenarioStartMethodExpression);

            var statementsWhenScenarioIsExecuted = new List<CodeStatement>
            {
                new CodeExpressionStatement(callScenarioStartMethodExpression)
            };


            if (generationContext.Feature.HasFeatureBackground())
            {
                using (new SourceLineScope(_specFlowConfiguration, _codeDomHelper, statementsWhenScenarioIsExecuted, generationContext.Document.SourceFilePath, generationContext.Feature.Background.Location))
                {
                    var backgroundMethodCallExpression = new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        generationContext.FeatureBackgroundMethod.Name);

                    _codeDomHelper.MarkCodeMethodInvokeExpressionAsAwait(backgroundMethodCallExpression);
                    statementsWhenScenarioIsExecuted.Add(new CodeExpressionStatement(backgroundMethodCallExpression));
                }
            }

            _scenarioPartHelper.GenerateRuleBackgroundStepsApplicableForThisScenario(generationContext, scenarioDefinition, statementsWhenScenarioIsExecuted);

            foreach (var scenarioStep in scenario.Steps)
            {
                _scenarioPartHelper.GenerateStep(generationContext, statementsWhenScenarioIsExecuted, scenarioStep, paramToIdentifier);
            }

            var tagsOfScenarioVariableReferenceExpression = new CodeVariableReferenceExpression(GeneratorConstants.SCENARIO_TAGS_VARIABLE_NAME);
            var featureFileTagFieldReferenceExpression = new CodeFieldReferenceExpression(null, GeneratorConstants.FEATURE_TAGS_VARIABLE_NAME);

            var tagHelperReference = new CodeTypeReferenceExpression(nameof(TagHelper));
            var scenarioTagIgnoredCheckStatement = new CodeMethodInvokeExpression(tagHelperReference, nameof(TagHelper.ContainsIgnoreTag), tagsOfScenarioVariableReferenceExpression);
            var featureTagIgnoredCheckStatement = new CodeMethodInvokeExpression(tagHelperReference, nameof(TagHelper.ContainsIgnoreTag), featureFileTagFieldReferenceExpression);

            var ifIsIgnoredStatement = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    scenarioTagIgnoredCheckStatement,
                    CodeBinaryOperatorType.BooleanOr,
                    featureTagIgnoredCheckStatement),
                statementsWhenScenarioIsIgnored,
                statementsWhenScenarioIsExecuted.ToArray()
                );

            testMethod.Statements.Add(ifIsIgnoredStatement);
        }

        internal void GenerateScenarioInitializeCall(TestClassGenerationContext generationContext, StepsContainer scenario, CodeMemberMethod testMethod)
        {
            var statements = new List<CodeStatement>();

            using (new SourceLineScope(_specFlowConfiguration, _codeDomHelper, statements, generationContext.Document.SourceFilePath, scenario.Location))
            {
                statements.Add(new CodeExpressionStatement(
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        generationContext.ScenarioInitializeMethod.Name,
                        new CodeVariableReferenceExpression("scenarioInfo"))));
            }

            testMethod.Statements.AddRange(statements.ToArray());
        }

        internal void GenerateScenarioCleanupMethodCall(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            // call scenario cleanup
            var expression = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                generationContext.ScenarioCleanupMethod.Name);

            _codeDomHelper.MarkCodeMethodInvokeExpressionAsAwait(expression);

            testMethod.Statements.Add(expression);
        }
        
        private CodeMethodInvokeExpression CreateTestRunnerSkipScenarioCall()
        {
            return new CodeMethodInvokeExpression(
                new CodeFieldReferenceExpression(null, TESTRUNNER_FIELD),
                nameof(TestRunner.SkipScenario));
        }

        private void GenerateScenarioOutlineExamplesAsIndividualMethods(
            ScenarioOutline scenarioOutline,
            TestClassGenerationContext generationContext,
            CodeMemberMethod scenarioOutlineTestMethod, ParameterSubstitution paramToIdentifier)
        {
            var exampleSetIndex = 0;

            foreach (var exampleSet in scenarioOutline.Examples)
            {
                var useFirstColumnAsName = CanUseFirstColumnAsName(exampleSet.TableBody);
                string exampleSetIdentifier;

                if (string.IsNullOrEmpty(exampleSet.Name))
                {
                    if (scenarioOutline.Examples.Count(es => string.IsNullOrEmpty(es.Name)) > 1)
                    {
                        exampleSetIdentifier = $"ExampleSet {exampleSetIndex}".ToIdentifier();
                    }
                    else
                    {
                        exampleSetIdentifier = null;
                    }
                }
                else
                {
                    exampleSetIdentifier = exampleSet.Name.ToIdentifier();
                }


                foreach (var example in exampleSet.TableBody.Select((r, i) => new { Row = r, Index = i }))
                {
                    var variantName = useFirstColumnAsName ? example.Row.Cells.First().Value : string.Format("Variant {0}", example.Index);
                    GenerateScenarioOutlineTestVariant(generationContext, scenarioOutline, scenarioOutlineTestMethod, paramToIdentifier, exampleSet.Name ?? "", exampleSetIdentifier, example.Row, exampleSet.Tags.ToArray(), variantName);
                }

                exampleSetIndex++;
            }
        }

        private void GenerateScenarioOutlineExamplesAsRowTests(TestClassGenerationContext generationContext, ScenarioOutline scenarioOutline, CodeMemberMethod scenatioOutlineTestMethod)
        {
            SetupTestMethod(generationContext, scenatioOutlineTestMethod, scenarioOutline, null, null, null, true);

            foreach (var examples in scenarioOutline.Examples)
            {
                foreach (var row in examples.TableBody)
                {
                    var arguments = row.Cells.Select(c => c.Value);
                    _unitTestGeneratorProvider.SetRow(generationContext, scenatioOutlineTestMethod, arguments, GetNonIgnoreTags(examples.Tags), HasIgnoreTag(examples.Tags));
                }
            }
        }

        private ParameterSubstitution CreateParamToIdentifierMapping(ScenarioOutline scenarioOutline)
        {
            var paramToIdentifier = new ParameterSubstitution();
            foreach (var param in scenarioOutline.Examples.First().TableHeader.Cells)
            {
                paramToIdentifier.Add(param.Value, param.Value.ToIdentifierCamelCase());
            }

            //fix empty parameters
            var emptyStrings = paramToIdentifier.Where(kv => kv.Value == string.Empty).ToArray();
            foreach (var item in emptyStrings)
            {
                paramToIdentifier.Remove(item);
                paramToIdentifier.Add(item.Key, "_");
            }

            //fix duplicated parameter names
            for (int i = 0; i < paramToIdentifier.Count; i++)
            {
                int suffix = 1;
                while (paramToIdentifier.Take(i).Count(kv => kv.Value == paramToIdentifier[i].Value) > 0)
                {
                    paramToIdentifier[i] = new KeyValuePair<string, string>(paramToIdentifier[i].Key, paramToIdentifier[i].Value + suffix);
                    suffix++;
                }
            }


            return paramToIdentifier;
        }


        private bool CanUseFirstColumnAsName(IEnumerable<Gherkin.Ast.TableRow> tableBody)
        {
            var tableBodyArray = tableBody.ToArray();
            if (tableBodyArray.Any(r => !r.Cells.Any()))
            {
                return false;
            }

            return tableBodyArray.Select(r => r.Cells.First().Value.ToIdentifier()).Distinct().Count() == tableBodyArray.Length;
        }

        private CodeMemberMethod CreateScenarioOutlineTestMethod(TestClassGenerationContext generationContext, ScenarioOutline scenarioOutline, ParameterSubstitution paramToIdentifier)
        {
            var testMethod = _codeDomHelper.CreateMethod(generationContext.TestClass);

            testMethod.Attributes = MemberAttributes.Public;
            testMethod.Name = string.Format(GeneratorConstants.TEST_NAME_FORMAT, scenarioOutline.Name.ToIdentifier());

            _codeDomHelper.MarkCodeMemberMethodAsAsync(testMethod);
            
            foreach (var pair in paramToIdentifier)
            {
                testMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), pair.Value));
            }

            testMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string[]), GeneratorConstants.SCENARIO_OUTLINE_EXAMPLE_TAGS_PARAMETER));
            return testMethod;
        }

        private void GenerateScenarioOutlineTestVariant(
            TestClassGenerationContext generationContext,
            ScenarioOutline scenarioOutline,
            CodeMemberMethod scenarioOutlineTestMethod,
            IEnumerable<KeyValuePair<string, string>> paramToIdentifier,
            string exampleSetTitle,
            string exampleSetIdentifier,
            Gherkin.Ast.TableRow row,
            Tag[] exampleSetTags,
            string variantName)
        {
            var testMethod = CreateTestMethod(generationContext, scenarioOutline, exampleSetTags, variantName, exampleSetIdentifier);
            
            //call test implementation with the params
            var argumentExpressions = row.Cells.Select(paramCell => new CodePrimitiveExpression(paramCell.Value)).Cast<CodeExpression>().ToList();

            argumentExpressions.Add(_scenarioPartHelper.GetStringArrayExpression(exampleSetTags));
            
            var statements = new List<CodeStatement>();

            using (new SourceLineScope(_specFlowConfiguration, _codeDomHelper, statements, generationContext.Document.SourceFilePath, scenarioOutline.Location))
            {
                var callTestMethodExpression = new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    scenarioOutlineTestMethod.Name,
                    argumentExpressions.ToArray());

                _codeDomHelper.MarkCodeMethodInvokeExpressionAsAwait(callTestMethodExpression);

                statements.Add(new CodeExpressionStatement(callTestMethodExpression));
            }
            
            testMethod.Statements.AddRange(statements.ToArray());

            //_linePragmaHandler.AddLineDirectiveHidden(testMethod.Statements);
            var arguments = paramToIdentifier.Select((pToId, paramIndex) => new KeyValuePair<string, string>(pToId.Key, row.Cells.ElementAt(paramIndex).Value)).ToList();

            // Use the identifier of the example set (e.g. ExampleSet0, ExampleSet1) if we have it.
            // Otherwise, use the title of the example set provided by the user in the feature file.
            string exampleSetName = string.IsNullOrEmpty(exampleSetIdentifier) ? exampleSetTitle : exampleSetIdentifier;
            _unitTestGeneratorProvider.SetTestMethodAsRow(generationContext, testMethod, scenarioOutline.Name, exampleSetName, variantName, arguments);
        }

        private CodeMemberMethod CreateTestMethod(
            TestClassGenerationContext generationContext,
            StepsContainer scenario,
            IEnumerable<Tag> additionalTags,
            string variantName = null,
            string exampleSetIdentifier = null)
        {
            var testMethod = _codeDomHelper.CreateMethod(generationContext.TestClass);
            _codeDomHelper.MarkCodeMemberMethodAsAsync(testMethod);

            SetupTestMethod(generationContext, testMethod, scenario, additionalTags, variantName, exampleSetIdentifier);

            return testMethod;
        }

        private void SetupTestMethod(
            TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod,
            StepsContainer scenarioDefinition,
            IEnumerable<Tag> additionalTags,
            string variantName,
            string exampleSetIdentifier,
            bool rowTest = false)
        {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            testMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            testMethod.Name = GetTestMethodName(scenarioDefinition, variantName, exampleSetIdentifier);
            var friendlyTestName = scenarioDefinition.Name;
            if (variantName != null)
            {
                friendlyTestName = $"{scenarioDefinition.Name}: {variantName}";
            }

            if (rowTest)
            {
                _unitTestGeneratorProvider.SetRowTest(generationContext, testMethod, friendlyTestName);
            }
            else
            {
                _unitTestGeneratorProvider.SetTestMethod(generationContext, testMethod, friendlyTestName);
            }

            _decoratorRegistry.DecorateTestMethod(generationContext, testMethod, ConcatTags(scenarioDefinition.GetTags(), additionalTags), out var scenarioCategories);

            if (scenarioCategories.Any())
            {
                _unitTestGeneratorProvider.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);
            }
        }

        private static string GetTestMethodName(StepsContainer scenario, string variantName, string exampleSetIdentifier)
        {
            var methodName = string.Format(GeneratorConstants.TEST_NAME_FORMAT, scenario.Name.ToIdentifier());
            if (variantName == null)
            {
                return methodName;
            }

            var variantNameIdentifier = variantName.ToIdentifier().TrimStart('_');
            methodName = string.IsNullOrEmpty(exampleSetIdentifier)
                ? $"{methodName}_{variantNameIdentifier}"
                : $"{methodName}_{exampleSetIdentifier}_{variantNameIdentifier}";

            return methodName;
        }

        private IEnumerable<Tag> ConcatTags(params IEnumerable<Tag>[] tagLists)
        {
            return tagLists.Where(tagList => tagList != null).SelectMany(tagList => tagList);
        }
    }
}