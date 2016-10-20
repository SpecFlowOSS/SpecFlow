using System;
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
        public void ShouldTraceWarningWhenCleanedUpWithoutBeingInitialized()
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
        private static IContextManager ResolveContextManager(ITestTracer testTracer)
        {
            var container = CreateObjectContainer(testTracer);
            var contextManager = container.Resolve<IContextManager>();
            return contextManager;
        }

        /// <summary>
        /// Creates an object container and registers the provided test tracer.
        /// </summary>
        /// <param name="testTracer">The test tracer that will be registered.</param>
        /// <returns>An object that implements <see cref="IObjectContainer"/>.</returns>
        private static IObjectContainer CreateObjectContainer(ITestTracer testTracer)
        {
            IObjectContainer container;
            TestObjectFactories.CreateTestRunner(
                out container,
                objectContainer => objectContainer.RegisterInstanceAs<ITestTracer>(testTracer));
            return container;
        }

        [Test]
        public void ShouldNotTraceWarningWhenInitializedTwice()
        {
            var mockTracer = new Mock<ITestTracer>();
            var contextManager = ResolveContextManager(mockTracer.Object);

            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty));
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialize twice", null, string.Empty));

            mockTracer.Verify(x => x.TraceWarning(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void ShouldNotTraceWarningWhenInitializedTwiceThenDisposedTwice()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty));
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialize twice", null, string.Empty));
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();
            mockTracer.Verify(x => x.TraceWarning(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void ShouldTraceWarningWhenInitializedTwiceThenCleanedUp3Times()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty));
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialize twice", null, string.Empty));
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();
            mockTracer.Verify(x => x.TraceWarning("The previous ScenarioStepContext was already disposed."),Times.Once());
        }

        [Test]
        public void ShouldReportCorrectCurrentStep()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once",null,string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            Assert.AreEqual(firstStepInfo,contextManager.StepContext.StepInfo);
            var secondStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize twice", null, string.Empty);
            contextManager.InitializeStepContext(secondStepInfo);
            Assert.AreEqual(secondStepInfo, contextManager.StepContext.StepInfo);
            contextManager.CleanupStepContext();
            Assert.AreEqual(firstStepInfo, contextManager.StepContext.StepInfo);
            contextManager.CleanupStepContext();
            Assert.AreEqual(null, contextManager.StepContext);
        }

        [Test]
        public void ShouldReportCorrectCurrentTopLevelStep()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            Assert.AreEqual(StepDefinitionType.Given, contextManager.CurrentTopLevelStepDefinitionType); // firstStepInfo
            var secondStepInfo = new StepInfo(StepDefinitionType.When, "I have called initialize twice", null, string.Empty);
            contextManager.InitializeStepContext(secondStepInfo);
            Assert.AreEqual(StepDefinitionType.Given, contextManager.CurrentTopLevelStepDefinitionType); // firstStepInfo
            contextManager.CleanupStepContext(); // remove second
            Assert.AreEqual(StepDefinitionType.Given, contextManager.CurrentTopLevelStepDefinitionType); // firstStepInfo
            contextManager.CleanupStepContext(); // remove first
            Assert.AreEqual(StepDefinitionType.Given, contextManager.CurrentTopLevelStepDefinitionType); // firstStepInfo
        }

        [Test]
        public void ShouldReportCorrectCurrentTopLevelStepIfWeHaveExecutedMoreThan1Step()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            Assert.AreEqual(StepDefinitionType.Given, contextManager.CurrentTopLevelStepDefinitionType); // firstStepInfo
            contextManager.CleanupStepContext();
            var secondStepInfo = new StepInfo(StepDefinitionType.When, "I have called initialize twice", null, string.Empty);
            contextManager.InitializeStepContext(secondStepInfo);
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
            contextManager.CleanupStepContext();
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
        }

        [Test]
        public void ShouldReportCorrectCurrentTopLevelStepIfWeHaveStepsMoreThan1Deep()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            Assert.AreEqual(StepDefinitionType.Given, contextManager.CurrentTopLevelStepDefinitionType); // firstStepInfo
            contextManager.CleanupStepContext();
            var secondStepInfo = new StepInfo(StepDefinitionType.When, "I have called initialize twice", null, string.Empty);
            contextManager.InitializeStepContext(secondStepInfo);
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
            var thirdStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize a third time", null, string.Empty);
            contextManager.InitializeStepContext(thirdStepInfo); //Call sub step
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
            var fourthStepInfo = new StepInfo(StepDefinitionType.Then, "I have called initialize a forth time", null, string.Empty);
            contextManager.InitializeStepContext(fourthStepInfo); //call sub step of sub step
            contextManager.CleanupStepContext(); // return from sub step of sub step
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
            contextManager.CleanupStepContext(); // return from sub step
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
            contextManager.CleanupStepContext(); // finish 2nd step
            Assert.AreEqual(StepDefinitionType.When, contextManager.CurrentTopLevelStepDefinitionType); // secondStepInfo
            var fifthStepInfo = new StepInfo(StepDefinitionType.Then, "I have called initialize a fifth time", null, string.Empty);
            contextManager.InitializeStepContext(fifthStepInfo);
            Assert.AreEqual(StepDefinitionType.Then, contextManager.CurrentTopLevelStepDefinitionType); // fifthStepInfo
        }

        [Test]
        public void TopLevelStepShouldBeNullInitially()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            Assert.IsNull(contextManager.CurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void ScenarioStartShouldResetTopLevelStep()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            // do not call CleanupStepContext to simulate inconsistent state

            contextManager.InitializeScenarioContext(new ScenarioInfo("my scenario"));

            Assert.IsNull(contextManager.CurrentTopLevelStepDefinitionType);
        }

        [Test]
        public void ShouldBeAbleToDisposeContextManagerAfterAnConsistentState()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            // do not call CleanupStepContext to simulate inconsistent state

            ((IDisposable)contextManager).Dispose();
        }

        [Test]
        public void ShouldBeAbleToDisposeContextManagerAfterAnInconsistentState()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestObjectFactories.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialize once", null, string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            contextManager.CleanupStepContext();

            ((IDisposable)contextManager).Dispose();
        }

        [Test]
        public void ShouldReportSetValuesCorrectly()
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