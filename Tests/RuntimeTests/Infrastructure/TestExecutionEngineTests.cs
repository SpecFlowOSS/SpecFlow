using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class TestExecutionEngineTests
    {
        private ScenarioContext scenarioContext;
        private RuntimeConfiguration runtimeConfiguration;
        private Mock<IBindingRegistry> bindingRegistryStub;
        private Mock<IErrorProvider> errorProviderStub;
        private Mock<IContextManager> contextManagerStub;
        private Mock<ITestTracer> testTracerStub;
        private Mock<IStepDefinitionMatcher> stepDefinitionMatcherStub;

        private readonly List<IHookBinding> beforeStepEvents = new List<IHookBinding>();
        private readonly List<IHookBinding> afterStepEvents = new List<IHookBinding>();
        private readonly List<IHookBinding> beforeScenarioBlockEvents = new List<IHookBinding>();
        private readonly List<IHookBinding> afterScenarioBlockEvents = new List<IHookBinding>();

        private TestExecutionEngine CreateTestExecutionEngine()
        {
            Dictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider> skeletonProviders = new Dictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider>();
            skeletonProviders.Add(ProgrammingLanguage.CSharp, new Mock<IStepDefinitionSkeletonProvider>().Object);

            var culture = new CultureInfo("en-US");
            contextManagerStub = new Mock<IContextManager>();
            scenarioContext = new ScenarioContext(new ScenarioInfo("scenario_title"), null, null);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);
            contextManagerStub.Setup(cm => cm.FeatureContext).Returns(new FeatureContext(new FeatureInfo(culture, "feature_title", "", ProgrammingLanguage.CSharp), culture));

            bindingRegistryStub = new Mock<IBindingRegistry>();
            bindingRegistryStub.Setup(br => br.GetEvents(BindingEvent.StepStart)).Returns(beforeStepEvents);
            bindingRegistryStub.Setup(br => br.GetEvents(BindingEvent.StepEnd)).Returns(afterStepEvents);
            bindingRegistryStub.Setup(br => br.GetEvents(BindingEvent.BlockStart)).Returns(beforeScenarioBlockEvents);
            bindingRegistryStub.Setup(br => br.GetEvents(BindingEvent.BlockEnd)).Returns(afterScenarioBlockEvents);

            runtimeConfiguration = new RuntimeConfiguration();
            errorProviderStub = new Mock<IErrorProvider>();
            testTracerStub = new Mock<ITestTracer>();
            stepDefinitionMatcherStub = new Mock<IStepDefinitionMatcher>();

            return new TestExecutionEngine(
                new Mock<IStepFormatter>().Object, 
                testTracerStub.Object, 
                errorProviderStub.Object, 
                new Mock<IStepArgumentTypeConverter>().Object, 
                runtimeConfiguration, 
                bindingRegistryStub.Object,
                new Mock<IUnitTestRuntimeProvider>().Object,
                skeletonProviders, 
                contextManagerStub.Object, 
                stepDefinitionMatcherStub.Object);
        }

        private Mock<IStepDefinitionBinding> RegisterStepDefinition()
        {
            var stepDefStub = new Mock<IStepDefinitionBinding>();
            stepDefinitionMatcherStub.Setup(sdm => sdm.GetMatches(It.IsAny<StepArgs>())).Returns((StepArgs sa) =>
                      new List<BindingMatch>() { new BindingMatch(stepDefStub.Object, sa, new string[0], new string[0], 0) });

            return stepDefStub;
        }


        private Mock<IHookBinding> CreateHookMock(List<IHookBinding> hookList)
        {
            var mock = new Mock<IHookBinding>();
            hookList.Add(mock.Object);
            return mock;
        }

        [Test]
        public void Should_execute_before_step()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(beforeStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, "foo", null, null);

            hookMock.Verify(sm => sm.Invoke(contextManagerStub.Object, testTracerStub.Object), Times.Once());
        }

        [Test]
        public void Should_execute_after_step()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, "foo", null, null);

            hookMock.Verify(sm => sm.Invoke(contextManagerStub.Object, testTracerStub.Object), Times.Once());
        }

        [Test]
        public void Should_not_execute_step_when_there_was_an_error_earlier()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            var stepDefMock = RegisterStepDefinition();

            scenarioContext.TestStatus = TestStatus.TestError;

            testExecutionEngine.Step(StepDefinitionKeyword.Given, "foo", null, null);

            TimeSpan duration;
            stepDefMock.Verify(sm => sm.Invoke(It.IsAny<IContextManager>(), It.IsAny<ITestTracer>(), It.IsAny<object[]>(), out duration), Times.Never());
        }

        [Test]
        public void Should_not_execute_step_hooks_when_there_was_an_error_earlier()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            scenarioContext.TestStatus = TestStatus.TestError;

            var beforeStepMock = CreateHookMock(beforeStepEvents);
            var afterStepMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, "foo", null, null);

            beforeStepMock.Verify(sm => sm.Invoke(It.IsAny<IContextManager>(), It.IsAny<ITestTracer>()), Times.Never());
            afterStepMock.Verify(sm => sm.Invoke(It.IsAny<IContextManager>(), It.IsAny<ITestTracer>()), Times.Never());
        }

        [Test]
        public void Should_execute_after_step_when_step_definition_failed()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            var stepDefStub = RegisterStepDefinition();
            TimeSpan duration;
            stepDefStub.Setup(sd => sd.Invoke(contextManagerStub.Object, testTracerStub.Object, It.IsAny<object[]>(), out duration))
                .Throws(new Exception("simulated error"));

            var hookMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, "foo", null, null);

            hookMock.Verify(sm => sm.Invoke(contextManagerStub.Object, testTracerStub.Object));
        }
    }
}
