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
        public void ShouldTraceWarningWhenCleanedUpWithoutBeingInitialised()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestTestRunnerFactory.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            contextManager.CleanupStepContext();
            mockTracer.Verify(x => x.TraceWarning("The previous ScenarioStepContext was already disposed."));
        }

        [Test]
        public void ShouldNotTraceWarningWhenInitialisedTwice()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestTestRunnerFactory.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialise once", null, string.Empty));
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialise twice", null, string.Empty));
            mockTracer.Verify(x => x.TraceWarning(It.IsAny<String>()), Times.Never());
        }

        [Test]
        public void ShouldNotTraceWarningWhenInitialisedTwiceThenDisposedTwice()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestTestRunnerFactory.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialise once", null, string.Empty));
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialise twice", null, string.Empty));
            contextManager.CleanupStepContext();
            contextManager.CleanupStepContext();
            mockTracer.Verify(x => x.TraceWarning(It.IsAny<String>()), Times.Never());
        }

        [Test]
        public void ShouldTraceWarningWhenInitialisedTwiceThenCleanedUp3Times()
        {
            IObjectContainer container;
            var mockTracer = new Mock<ITestTracer>();
            TestTestRunnerFactory.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialise once", null, string.Empty));
            contextManager.InitializeStepContext(new StepInfo(StepDefinitionType.Given, "I have called initialise twice", null, string.Empty));
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
            TestTestRunnerFactory.CreateTestRunner(out container, objectContainer => objectContainer.RegisterInstanceAs(mockTracer.Object));
            var contextManager = container.Resolve<IContextManager>();
            var firstStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialise once",null,string.Empty);
            contextManager.InitializeStepContext(firstStepInfo);
            Assert.AreEqual(firstStepInfo,contextManager.StepContext.StepInfo);
            var secondStepInfo = new StepInfo(StepDefinitionType.Given, "I have called initialise twice", null, string.Empty);
            contextManager.InitializeStepContext(secondStepInfo);
            Assert.AreEqual(secondStepInfo, contextManager.StepContext.StepInfo);
            contextManager.CleanupStepContext();
            Assert.AreEqual(firstStepInfo, contextManager.StepContext.StepInfo);
            contextManager.CleanupStepContext();
            Assert.AreEqual(null, contextManager.StepContext);            
        }

        [Test]
        public void ShouldReportSetValuesCorrectly()
        {
            var table = new Table("header1","header2");
            const string multlineText = @" some
eaxmplae
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