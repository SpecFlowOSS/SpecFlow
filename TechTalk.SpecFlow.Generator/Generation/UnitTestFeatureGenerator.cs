using System;
using System.CodeDom;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Generator.Generation
{
    public class UnitTestFeatureGenerator : IFeatureGenerator
    {
        private readonly CodeDomHelper _codeDomHelper;
        private readonly IDecoratorRegistry _decoratorRegistry;
        private readonly ScenarioPartHelper _scenarioPartHelper;
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly IUnitTestGeneratorProvider _testGeneratorProvider;
        private readonly UnitTestMethodGenerator _unitTestMethodGenerator;
        private readonly LinePragmaHandler _linePragmaHandler;

        public UnitTestFeatureGenerator(
            IUnitTestGeneratorProvider testGeneratorProvider,
            CodeDomHelper codeDomHelper,
            SpecFlowConfiguration specFlowConfiguration,
            IDecoratorRegistry decoratorRegistry)
        {
            _testGeneratorProvider = testGeneratorProvider;
            _codeDomHelper = codeDomHelper;
            _specFlowConfiguration = specFlowConfiguration;
            _decoratorRegistry = decoratorRegistry;
            _linePragmaHandler = new LinePragmaHandler(_specFlowConfiguration, _codeDomHelper);
            _scenarioPartHelper = new ScenarioPartHelper(_specFlowConfiguration, _codeDomHelper);
            _unitTestMethodGenerator = new UnitTestMethodGenerator(testGeneratorProvider, decoratorRegistry, _codeDomHelper, _scenarioPartHelper, _specFlowConfiguration);
        }

        public string TestClassNameFormat { get; set; } = "{0}Feature";

        public CodeNamespace GenerateUnitTestFixture(SpecFlowDocument document, string testClassName, string targetNamespace)
        {
            var codeNamespace = CreateNamespace(targetNamespace);
            var feature = document.SpecFlowFeature;

            testClassName = testClassName ?? string.Format(TestClassNameFormat, feature.Name.ToIdentifier());
            var generationContext = CreateTestClassStructure(codeNamespace, testClassName, document);

            SetupTestClass(generationContext);
            SetupTestClassInitializeMethod(generationContext);
            SetupTestClassCleanupMethod(generationContext);

            SetupScenarioStartMethod(generationContext);
            SetupScenarioInitializeMethod(generationContext);
            _scenarioPartHelper.SetupFeatureBackground(generationContext);
            SetupScenarioCleanupMethod(generationContext);

            SetupTestInitializeMethod(generationContext);
            SetupTestCleanupMethod(generationContext);

            _unitTestMethodGenerator.CreateUnitTests(feature, generationContext);

            //before returning the generated code, call the provider's method in case the generated code needs to be customized            
            _testGeneratorProvider.FinalizeTestClass(generationContext);
            return codeNamespace;
        }


        private TestClassGenerationContext CreateTestClassStructure(CodeNamespace codeNamespace, string testClassName, SpecFlowDocument document)
        {
            var testClass = _codeDomHelper.CreateGeneratedTypeDeclaration(testClassName);
            codeNamespace.Types.Add(testClass);

            return new TestClassGenerationContext(
                _testGeneratorProvider,
                document,
                codeNamespace,
                testClass,
                DeclareTestRunnerMember(testClass),
                _codeDomHelper.CreateMethod(testClass),
                _codeDomHelper.CreateMethod(testClass),
                _codeDomHelper.CreateMethod(testClass),
                _codeDomHelper.CreateMethod(testClass),
                _codeDomHelper.CreateMethod(testClass),
                _codeDomHelper.CreateMethod(testClass),
                _codeDomHelper.CreateMethod(testClass),
                document.SpecFlowFeature.HasFeatureBackground() ? _codeDomHelper.CreateMethod(testClass) : null,
                _testGeneratorProvider.GetTraits().HasFlag(UnitTestGeneratorTraits.RowTests) && _specFlowConfiguration.AllowRowTests);
        }

        private CodeNamespace CreateNamespace(string targetNamespace)
        {
            targetNamespace = targetNamespace ?? GeneratorConstants.DEFAULT_NAMESPACE;

            if (!targetNamespace.StartsWith("global", StringComparison.CurrentCultureIgnoreCase))
            {
                switch (_codeDomHelper.TargetLanguage)
                {
                    case CodeDomProviderLanguage.VB:
                        targetNamespace = $"GlobalVBNetNamespace.{targetNamespace}";
                        break;
                }
            }

            var codeNamespace = new CodeNamespace(targetNamespace);

            codeNamespace.Imports.Add(new CodeNamespaceImport(GeneratorConstants.SPECFLOW_NAMESPACE));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            return codeNamespace;
        }

        private void SetupScenarioCleanupMethod(TestClassGenerationContext generationContext)
        {
            var scenarioCleanupMethod = generationContext.ScenarioCleanupMethod;

            scenarioCleanupMethod.Attributes = MemberAttributes.Public;
            scenarioCleanupMethod.Name = GeneratorConstants.SCENARIO_CLEANUP_NAME;

            // call collect errors
            var testRunnerField = _scenarioPartHelper.GetTestRunnerExpression();

            //testRunner.CollectScenarioErrors();
            scenarioCleanupMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "CollectScenarioErrors"));
        }

        private void SetupTestClass(TestClassGenerationContext generationContext)
        {
            generationContext.TestClass.IsPartial = true;
            generationContext.TestClass.TypeAttributes |= TypeAttributes.Public;

            _linePragmaHandler.AddLinePragmaInitial(generationContext.TestClass, generationContext.Document.SourceFilePath);

            _testGeneratorProvider.SetTestClass(generationContext, generationContext.Feature.Name, generationContext.Feature.Description);

            _decoratorRegistry.DecorateTestClass(generationContext, out var featureCategories);

            if (featureCategories.Any())
            {
                _testGeneratorProvider.SetTestClassCategories(generationContext, featureCategories);
            }

            var featureTagsField = new CodeMemberField(typeof(string[]), GeneratorConstants.FEATURE_TAGS_VARIABLE_NAME);
            featureTagsField.InitExpression = _scenarioPartHelper.GetStringArrayExpression(generationContext.Feature.Tags);

            generationContext.TestClass.Members.Add(featureTagsField);
        }

        private CodeMemberField DeclareTestRunnerMember(CodeTypeDeclaration type)
        {
            var testRunnerField = new CodeMemberField(typeof(ITestRunner), GeneratorConstants.TESTRUNNER_FIELD);
            type.Members.Add(testRunnerField);
            return testRunnerField;
        }


        private void SetupTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            var testClassInitializeMethod = generationContext.TestClassInitializeMethod;

            testClassInitializeMethod.Attributes = MemberAttributes.Public;
            testClassInitializeMethod.Name = GeneratorConstants.TESTCLASS_INITIALIZE_NAME;

            _testGeneratorProvider.SetTestClassInitializeMethod(generationContext);

            //testRunner = TestRunnerManager.GetTestRunner(); if UnitTestGeneratorTraits.ParallelExecution
            //testRunner = TestRunnerManager.GetTestRunner(null, 0); if not UnitTestGeneratorTraits.ParallelExecution
            var testRunnerField = _scenarioPartHelper.GetTestRunnerExpression();

            var testRunnerParameters = _testGeneratorProvider.GetTraits().HasFlag(UnitTestGeneratorTraits.ParallelExecution)
                ? new CodeExpression[] { }
                : new[] {new CodePrimitiveExpression(null), new CodePrimitiveExpression(0)};

            testClassInitializeMethod.Statements.Add(
                new CodeAssignStatement(
                    testRunnerField,
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(TestRunnerManager)),
                        "GetTestRunner", testRunnerParameters)));

            //FeatureInfo featureInfo = new FeatureInfo("xxxx");
            testClassInitializeMethod.Statements.Add(
                new CodeVariableDeclarationStatement(typeof(FeatureInfo), "featureInfo",
                    new CodeObjectCreateExpression(typeof(FeatureInfo),
                        new CodeObjectCreateExpression(typeof(CultureInfo),
                            new CodePrimitiveExpression(generationContext.Feature.Language)),
                        new CodePrimitiveExpression(generationContext.Document.DocumentLocation?.FeatureFolderPath),
                        new CodePrimitiveExpression(generationContext.Feature.Name),
                        new CodePrimitiveExpression(generationContext.Feature.Description),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression("ProgrammingLanguage"),
                            _codeDomHelper.TargetLanguage.ToString()),
                        _scenarioPartHelper.GetStringArrayExpression(generationContext.Feature.Tags))));

            //testRunner.OnFeatureStart(featureInfo);
            testClassInitializeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnFeatureStart",
                    new CodeVariableReferenceExpression("featureInfo")));
        }


        private void SetupTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            var testClassCleanupMethod = generationContext.TestClassCleanupMethod;

            testClassCleanupMethod.Attributes = MemberAttributes.Public;
            testClassCleanupMethod.Name = GeneratorConstants.TESTCLASS_CLEANUP_NAME;

            _testGeneratorProvider.SetTestClassCleanupMethod(generationContext);

            var testRunnerField = _scenarioPartHelper.GetTestRunnerExpression();
            //            testRunner.OnFeatureEnd();
            testClassCleanupMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnFeatureEnd"));
            //            testRunner = null;
            testClassCleanupMethod.Statements.Add(
                new CodeAssignStatement(
                    testRunnerField,
                    new CodePrimitiveExpression(null)));
        }

        private void SetupTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            var testInitializeMethod = generationContext.TestInitializeMethod;

            testInitializeMethod.Attributes = MemberAttributes.Public;
            testInitializeMethod.Name = GeneratorConstants.TEST_INITIALIZE_NAME;

            _testGeneratorProvider.SetTestInitializeMethod(generationContext);
        }

        private void SetupTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            var testCleanupMethod = generationContext.TestCleanupMethod;

            testCleanupMethod.Attributes = MemberAttributes.Public;
            testCleanupMethod.Name = GeneratorConstants.TEST_CLEANUP_NAME;

            _testGeneratorProvider.SetTestCleanupMethod(generationContext);

            var testRunnerField = _scenarioPartHelper.GetTestRunnerExpression();
            //testRunner.OnScenarioEnd();
            testCleanupMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnScenarioEnd"));
        }

        private void SetupScenarioInitializeMethod(TestClassGenerationContext generationContext)
        {
            var scenarioInitializeMethod = generationContext.ScenarioInitializeMethod;

            scenarioInitializeMethod.Attributes = MemberAttributes.Public;
            scenarioInitializeMethod.Name = GeneratorConstants.SCENARIO_INITIALIZE_NAME;
            scenarioInitializeMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(ScenarioInfo), "scenarioInfo"));

            //testRunner.OnScenarioInitialize(scenarioInfo);
            var testRunnerField = _scenarioPartHelper.GetTestRunnerExpression();
            scenarioInitializeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    nameof(ITestExecutionEngine.OnScenarioInitialize),
                    new CodeVariableReferenceExpression("scenarioInfo")));
        }

        private void SetupScenarioStartMethod(TestClassGenerationContext generationContext)
        {
            var scenarioStartMethod = generationContext.ScenarioStartMethod;

            scenarioStartMethod.Attributes = MemberAttributes.Public;
            scenarioStartMethod.Name = GeneratorConstants.SCENARIO_START_NAME;

            //testRunner.OnScenarioStart();
            var testRunnerField = _scenarioPartHelper.GetTestRunnerExpression();
            scenarioStartMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    nameof(ITestExecutionEngine.OnScenarioStart)));
        }
    }
}