using System;
using System.Collections.Generic;
using System.Globalization;
using BoDi;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;


namespace TechTalk.SpecFlow.RuntimeTests
{
    public class StepExecutionTestsBase
    {
        protected CultureInfo FeatureLanguage;
        protected Mock<IStepArgumentTypeConverter> StepArgumentTypeConverterStub;
        protected ObjectContainer TestThreadContainer;

        protected IContextManager ContextManagerStub;
        protected IContainerBuilder ContainerBuilderStub;

        #region dummy test tracer
        public class DummyTestTracer : ITestTracer
        {
            public void TraceStep(StepInstance stepInstance, bool showAdditionalArguments)
            {
            }

            public void TraceWarning(string text)
            {
                Console.WriteLine("TraceWarning: {0}", text);
            }

            public void TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration)
            {
            }

            public void TraceStepSkipped()
            {
                Console.WriteLine("TraceStepSkipped");
            }

            public void TraceStepPending(BindingMatch match, object[] arguments)
            {
                Console.WriteLine("TraceStepPending");
            }

            public void TraceBindingError(BindingException ex)
            {
                Console.WriteLine("TraceBindingError: {0}", ex);
            }

            public void TraceError(Exception ex, TimeSpan duration)
            {
                Console.WriteLine("TraceError: {0}, ({1:F1}s)", ex, duration.TotalSeconds);
            }

            public void TraceNoMatchingStepDefinition(StepInstance stepInstance, ProgrammingLanguage targetLanguage, CultureInfo bindingCulture, List<BindingMatch> matchesWithoutScopeCheck)
            {
                Console.WriteLine("TraceNoMatchingStepDefinition");
            }

            public void TraceDuration(TimeSpan elapsed, IBindingMethod method, object[] arguments)
            {
                //nop
            }

            public void TraceDuration(TimeSpan elapsed, string text)
            {
                //nop
            }
        }
        #endregion

        protected virtual CultureInfo GetFeatureLanguage()
        {
            return new CultureInfo("en-US", false);
        }     
        
        protected virtual CultureInfo GetBindingCulture()
        {
            return new CultureInfo("en-US", false);
        }        

        public StepExecutionTestsBase()
        {
            TestRunnerManager.Reset();

            
            

            // FeatureContext and ScenarioContext is needed, because the [Binding]-instances live there
            FeatureLanguage = GetFeatureLanguage();
            var runtimeConfiguration = ConfigurationLoader.GetDefault();
            runtimeConfiguration.BindingCulture = GetBindingCulture();

            TestThreadContainer = new ObjectContainer();
            TestThreadContainer.RegisterInstanceAs(runtimeConfiguration);
            TestThreadContainer.RegisterInstanceAs(new Mock<ITestRunner>().Object);
            TestThreadContainer.RegisterTypeAs<TestObjectResolver, ITestObjectResolver>();
            var containerBuilderMock = new Mock<IContainerBuilder>();
            containerBuilderMock.Setup(m => m.CreateScenarioContainer(It.IsAny<IObjectContainer>(), It.IsAny<ScenarioInfo>()))
                .Returns((IObjectContainer fc, ScenarioInfo si) =>
                {
                    var scenarioContainer = new ObjectContainer(fc);
                    scenarioContainer.RegisterInstanceAs(si);
                    return scenarioContainer;
                });
            containerBuilderMock.Setup(m => m.CreateFeatureContainer(It.IsAny<IObjectContainer>(), It.IsAny<FeatureInfo>()))
                .Returns((IObjectContainer ttc, FeatureInfo fi) =>
                {
                    var featureContainer = new ObjectContainer(ttc);
                    featureContainer.RegisterInstanceAs(fi);
                    return featureContainer;
                });
            ContainerBuilderStub = containerBuilderMock.Object;


            ContextManagerStub = new ContextManager(new Mock<ITestTracer>().Object, TestThreadContainer, ContainerBuilderStub);
            ContextManagerStub.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, string.Empty, "test feature", null));
            ContextManagerStub.InitializeScenarioContext(new ScenarioInfo("test scenario", "test scenario description", null, null));

            StepArgumentTypeConverterStub = new Mock<IStepArgumentTypeConverter>();
        }

        protected TestRunner GetTestRunnerFor(params Type[] bindingTypes)
        {
            return GetTestRunnerFor(null, bindingTypes);
        }

        protected TestRunner GetTestRunnerFor(Action<IObjectContainer> registerMocks, params Type[] bindingTypes)
        {
            return TestObjectFactories.CreateTestRunner(
                container =>
                    {
                        container.RegisterTypeAs<DummyTestTracer, ITestTracer>();
                        container.RegisterInstanceAs(ContextManagerStub);

                        var builder = (RuntimeBindingRegistryBuilder)container.Resolve<IRuntimeBindingRegistryBuilder>();
                        foreach (var bindingType in bindingTypes)
                            builder.BuildBindingsFromType(bindingType);

                        registerMocks?.Invoke(container);
                    });
        }

        protected (TestRunner, Mock<TBinding>) GetTestRunnerFor<TBinding>() where TBinding : class 
        {
            return GetTestRunnerWithConverterStub<TBinding>(null);
        }

        protected (TestRunner, Mock<TBinding>) GetTestRunnerWithConverterStub<TBinding>() where TBinding : class
        {
            return GetTestRunnerWithConverterStub<TBinding>(c => c.RegisterInstanceAs(StepArgumentTypeConverterStub.Object));
        }

        private (TestRunner, Mock<TBinding>) GetTestRunnerWithConverterStub<TBinding>(Action<IObjectContainer> registerMocks) where TBinding : class
        {
            TestRunner testRunner = GetTestRunnerFor(registerMocks, typeof(TBinding));

            var bindingInstance = new Mock<TBinding>();
            testRunner.ScenarioContext.SetBindingInstance(typeof(TBinding), bindingInstance.Object);
            return (testRunner, bindingInstance);
        }

        protected ScenarioExecutionStatus GetLastTestStatus()
        {
            return ContextManagerStub.ScenarioContext.ScenarioExecutionStatus;
        }
    }
}