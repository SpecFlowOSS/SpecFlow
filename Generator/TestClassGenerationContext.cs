using System.CodeDom;
using System.Collections.Generic;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator
{
    public class TestClassGenerationContext
    {
        public IUnitTestGeneratorProvider UnitTestGeneratorProvider { get; private set; }
        public Feature Feature { get; private set; }

        public CodeNamespace Namespace { get; private set; }
        public CodeTypeDeclaration TestClass { get; private set; }
        public CodeMemberMethod TestClassInitializeMethod { get; private set; }
        public CodeMemberMethod TestClassCleanupMethod { get; private set; }
        public CodeMemberMethod TestInitializeMethod { get; private set; }
        public CodeMemberMethod TestCleanupMethod { get; private set; }
        public CodeMemberMethod ScenarioInitializeMethod { get; private set; }
        public CodeMemberMethod ScenarioCleanupMethod { get; private set; }
        public CodeMemberMethod FeatureBackgroundMethod { get; private set; }

        public bool GenerateRowTests { get; private set; }
        public bool GenerateAsynchTests { get; private set; }

        public IDictionary<string, object> CustomData { get; private set; }

        public TestClassGenerationContext(IUnitTestGeneratorProvider unitTestGeneratorProvider, Feature feature, CodeNamespace ns, CodeTypeDeclaration testClass, CodeMemberMethod testClassInitializeMethod, CodeMemberMethod testClassCleanupMethod, CodeMemberMethod testInitializeMethod, CodeMemberMethod testCleanupMethod, CodeMemberMethod scenarioInitializeMethod, CodeMemberMethod scenarioCleanupMethod, CodeMemberMethod featureBackgroundMethod, bool generateRowTests, bool generateAsynchTests)
        {
            UnitTestGeneratorProvider = unitTestGeneratorProvider;
            Feature = feature;
            Namespace = ns;
            TestClass = testClass;
            TestClassInitializeMethod = testClassInitializeMethod;
            TestClassCleanupMethod = testClassCleanupMethod;
            TestInitializeMethod = testInitializeMethod;
            TestCleanupMethod = testCleanupMethod;
            ScenarioInitializeMethod = scenarioInitializeMethod;
            ScenarioCleanupMethod = scenarioCleanupMethod;
            FeatureBackgroundMethod = featureBackgroundMethod;
            GenerateRowTests = generateRowTests;
            GenerateAsynchTests = generateAsynchTests;

            CustomData = new Dictionary<string, object>();
        }
    }
}