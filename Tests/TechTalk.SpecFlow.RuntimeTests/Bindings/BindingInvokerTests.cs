using System;
using System.Threading.Tasks;
using BoDi;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.RuntimeTests.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using Xunit;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings;

public class BindingInvokerTests
{
    private static ITestOutputHelper _testOutputHelperInstance;
    private readonly ITestOutputHelper _testOutputHelper;

    public BindingInvokerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _testOutputHelperInstance = _testOutputHelper; // to be able to access it from step def classes
    }

    private BindingInvoker CreateSut()
    {
        return new BindingInvoker(ConfigurationLoader.GetDefault(), new StubErrorProvider(), new BindingDelegateInvoker());
    }

    class SampleStepDefClass
    {
        public string CapturedValue = null;

        public void SampleSyncStepDef(int someParam)
        {
            _testOutputHelperInstance.WriteLine($"some info here: {someParam}");
        }

        public async Task SampleAsyncStepDef(string otherParam)
        {
            _testOutputHelperInstance.WriteLine($"some info here: {otherParam}");
            CapturedValue = otherParam;
        }
    }

    class TestableMethodBinding : MethodBinding
    {
        public TestableMethodBinding(IBindingMethod bindingMethod) : base(bindingMethod)
        {
        }
    }

    private async Task InvokeBindingAsync(BindingInvoker sut, IContextManager contextManager, Type stepDefType, string methodName, params object[] args)
    {
        var testTracerMock = new Mock<ITestTracer>();
        var setMethodBinding = new TestableMethodBinding(new RuntimeBindingMethod(stepDefType.GetMethod(methodName)));
        await sut.InvokeBindingAsync(setMethodBinding, contextManager, args, testTracerMock.Object, new DurationHolder());
    }

    private IContextManager CreateContextManagerWith()
    {
        var contextManagerMock = new Mock<IContextManager>();
        var scenarioContainer = new ObjectContainer();
        var scenarioContext = new ScenarioContext(scenarioContainer, new ScenarioInfo("S1", null, null, null), new TestObjectResolver());
        contextManagerMock.SetupGet(ctx => ctx.ScenarioContext).Returns(scenarioContext);
        var contextManager = contextManagerMock.Object;
        return contextManager;
    }

    [Fact]
    public async Task Sample_binding_invoker_test()
    {
        _testOutputHelper.WriteLine("starting sample test");
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        // call step definition methods
        await InvokeBindingAsync(sut, contextManager, typeof(SampleStepDefClass), nameof(SampleStepDefClass.SampleSyncStepDef), 42);
        await InvokeBindingAsync(sut, contextManager, typeof(SampleStepDefClass), nameof(SampleStepDefClass.SampleAsyncStepDef), "42");

        // this is how to get THE instance of the step definition class
        var stepDefClass = contextManager.ScenarioContext.ScenarioContainer.Resolve<SampleStepDefClass>();
        stepDefClass.CapturedValue.Should().Be("42");
    }

    #region ValueTask related tests

    class StepDefClassWithValueTask
    {
        public bool WasInvokedAsyncValueTaskStepDef = false;
        public bool WasInvokedAsyncValueTaskOfTStepDef = false;

        public async ValueTask AsyncValueTaskStepDef()
        {
            await Task.Delay(50); // we need to wait a bit otherwise the assertion passes even if the method is called sync
            WasInvokedAsyncValueTaskStepDef = true;
        }

        public async ValueTask<string> AsyncValueTaskOfTStepDef()
        {
            await Task.Delay(50); // we need to wait a bit otherwise the assertion passes even if the method is called sync
            WasInvokedAsyncValueTaskOfTStepDef = true;
            return "foo";
        }
    }

    [Fact]
    public async Task Async_methods_of_ValueTask_return_type_can_be_invoked()
    {
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        // call step definition methods
        await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithValueTask), nameof(StepDefClassWithValueTask.AsyncValueTaskStepDef));

        // this is how to get THE instance of the step definition class
        var stepDefClass = contextManager.ScenarioContext.ScenarioContainer.Resolve<StepDefClassWithValueTask>();
        stepDefClass.WasInvokedAsyncValueTaskStepDef.Should().BeTrue();
    }

    [Fact]
    public async Task Async_methods_of_ValueTaskOfT_return_type_can_be_invoked()
    {
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithValueTask), nameof(StepDefClassWithValueTask.AsyncValueTaskOfTStepDef));

        var stepDefClass = contextManager.ScenarioContext.ScenarioContainer.Resolve<StepDefClassWithValueTask>();
        stepDefClass.WasInvokedAsyncValueTaskOfTStepDef.Should().BeTrue();
    }

    #endregion
}