using System;
using System.Globalization;
using BoDi;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class ScenarioStepContextTests
    {
        [Test]
        public void CleanupStepContext_WhenNotInitialized_ShouldTraceWarning()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.CleanupStepContext();

            mockTracer.Verify(x => x.TraceWarning("The previous ScenarioStepContext was already disposed."));
        }

        /// <summary>
        /// Resolves the context manager and registers the provided test tracer.
        /// </summary>
        /// <param name="testTracer">The test tracer that will be registered.</param>
        /// <returns>An object that implements <see cref="IContextManager"/>.</returns>
        private IContextManager ResolveContextManager(ITestTracer testTracer)
        {
            var container = this.CreateTestThreadObjectContainer(testTracer);
            var contextManager = container.Resolve<IContextManager>();
            return contextManager;
        }

        /// <summary>
        /// Creates a test thread object container and registers the provided test tracer.
        /// </summary>
        /// <param name="testTracer">The test tracer that will be registered.</param>
        /// <returns>An object that implements <see cref="IObjectContainer"/>.</returns>
        private IObjectContainer CreateTestThreadObjectContainer(ITestTracer testTracer)
        {
            IObjectContainer container;
            TestObjectFactories.CreateTestRunner(
                out container,
                objectContainer => objectContainer.RegisterInstanceAs<ITestTracer>(testTracer));
            return container;
        }

        [Test]
        public void InitializeStepContext_WhenInitializedTwice_ShouldNotTraceWarning()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice"));

            mockTracer.Verify(x => x.TraceWarning(It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Creates a <see cref="StepInfo" /> instance, with the specified text and optional step definition type.
        /// </summary>
        /// <param name="text">The text of the step.</param>
        /// <param name="stepDefinitionType">The type of the step definition. Default value is StepDefinitionType.Given</param>
        /// <returns>A new instance of the <see cref="StepInfo"/> class.</returns>
        private StepInfo CreateStepInfo(string text, StepDefinitionType stepDefinitionType = StepDefinitionType.Given)
        {
            return new StepInfo(stepDefinitionType, text, null, string.Empty);
        }

        [Test]
        public void CleanupStepContext_WhenInitializedAsOftenAsCleanedUp_ShouldNotTraceWarning()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice"));
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();

            mockTracer.Verify(x => x.TraceWarning(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void CleanupStepContext_WhenCleanedUpMoreOftenThanInitialized_ShouldTraceWarning()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);


            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice"));

            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();

            mockTracer.Verify(x => x.TraceWarning("The previous ScenarioStepContext was already disposed."), Times.Once());
        }

        [Test]
        public void StepContext_WhenInitializedOnce_ShouldReportStepInfo()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            var firstStepInfo = CreateStepInfo("I have called initialize once");
            contextManager.InitializeStepContext(firstStepInfo);

            var actualStepInfo = contextManager.StepContext.StepInfo;

            Assert.AreEqual(firstStepInfo, actualStepInfo);
        }

        [Test]
        public void StepContext_WhenInitializedTwice_ShouldReportSecondStepInfo()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            var secondStepInfo = CreateStepInfo("I have called initialize twice");
            contextManager.InitializeStepContext(secondStepInfo);

            var actualStepInfo = contextManager.StepContext.StepInfo;

            Assert.AreEqual(secondStepInfo, actualStepInfo);
        }

        [Test]
        public void StepContext_WhenInitializedTwiceAndCleanedUpOnce_ShouldReportFirstStepInfo()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            var firstStepInfo = CreateStepInfo("I have called initialize once");
            contextManager.InitializeStepContext(firstStepInfo);
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice"));
            contextManager.CleanupStepContext();

            var actualStepInfo = contextManager.StepContext.StepInfo;

            Assert.AreEqual(firstStepInfo, actualStepInfo);
        }

        [Test]
        public void StepContext_WhenInitializedTwiceAndCleanedUpTwice_ShouldReportNoStepInfo()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice"));
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();

            Assert.AreEqual(null, contextManager.StepContext);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_WithoutInitialization_ShouldReportNull()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            Assert.IsNull(contextManager.CurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_WhenInitialized_ShouldReportCorrectStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.Given, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_WhenInitializedTwice_ShouldReportFirstStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice", StepDefinitionType.When));

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.Given, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_WhenInitializedTwiceAndCleanedUpOnce_ShouldReportFirstStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice", StepDefinitionType.When));
            contextManager.CleanupStepContext(); // remove second

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.Given, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_WhenInitializedTwiceAndCleanedUpTwice_ShouldReportFirstStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice", StepDefinitionType.When));
            contextManager.CleanupStepContext(); // remove second
            contextManager.CleanupStepContext(); // remove first

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.Given, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_AfterInitializationAndCleanupAndNewInitialization_ShouldReportSecondStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.CleanupStepContext();
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice", StepDefinitionType.When));

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.When, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_AfterInitializationAndCleanupAndNewInitializationAndCleanup_ShouldReportSecondStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize once"));
            contextManager.CleanupStepContext();
            contextManager.InitializeStepContext(CreateStepInfo("I have called initialize twice", StepDefinitionType.When));
            contextManager.CleanupStepContext();

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.When, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_AfterInitializingSubStepThatHasASubStepItselfAndCompletingDeepestLevelOfSteps_ShouldReportOriginalStepType()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(this.CreateStepInfo("I have called initialize once", StepDefinitionType.Given));
            contextManager.InitializeStepContext(this.CreateStepInfo("I have called initialize a second time, to initialize a sub step", StepDefinitionType.When));
            contextManager.InitializeStepContext(this.CreateStepInfo("I have called initialize a third time, to initialize a sub step of a sub step", StepDefinitionType.Then));
            contextManager.CleanupStepContext(); // return from sub step of sub step

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.AreEqual(StepDefinitionType.Given, actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void CurrentTopLevelStepDefinitionType_AfterInitializingNewScenarioContext_ShouldReportNull()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);
            contextManager.InitializeFeatureContext(new FeatureInfo(new CultureInfo("en-US"), "F", null));

            contextManager.InitializeStepContext(this.CreateStepInfo("I have called initialize once"));
            //// Do not call CleanupStepContext, in order to simulate an inconsistent state
            contextManager.InitializeScenarioContext(new ScenarioInfo("the next scenario"));

            var actualCurrentTopLevelStepDefinitionType = contextManager.CurrentTopLevelStepDefinitionType;

            Assert.IsNull(actualCurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void Dispose_InInconsistentState_ShouldNotThrowException()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(this.CreateStepInfo("I have called initialize once"));
            //// do not call CleanupStepContext to simulate inconsistent state

            Assert.DoesNotThrow(() => ((IDisposable)contextManager).Dispose());
        }

        [Test]
        public void Dispose_InConsistentState_ShouldNotThrowException()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(this.CreateStepInfo("I have called initialize once"));
            contextManager.CleanupStepContext();

            Assert.DoesNotThrow(() => ((IDisposable)contextManager).Dispose());
        }

        [Test]
        public void StepInfoConstructor_WithValidValues_ShouldReportThoseValuesCorrectly()
        {
            var table = new Table("header1","header2");
            const string multlineText = @" some
example
multiline
text";

            var stepInfo = new StepInfo(StepDefinitionType.Given, "Step text", table, multlineText);

            stepInfo.StepDefinitionType.Should().Be(StepDefinitionType.Given);
            stepInfo.Text.Should().Be("Step text");
            stepInfo.Table.Should().Be(table);
            stepInfo.MultilineText.Should().Be(multlineText);
        }
    }
}