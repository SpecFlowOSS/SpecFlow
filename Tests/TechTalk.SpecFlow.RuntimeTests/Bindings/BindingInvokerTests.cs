using System;
using System.Runtime.CompilerServices;
using System.Threading;
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

    #region Async void related tests

    class StepDefClassWithAsyncVoid
    {
        public static bool WasInvokedAsyncVoidStepDef = false;

        public async void AsyncVoidStepDef()
        {
            await Task.Delay(50); // we need to wait a bit otherwise the assertion passes even if the method is called sync
            WasInvokedAsyncVoidStepDef = true;
        }
    }

    [Fact]
    public async Task Async_void_binding_methods_are_not_supported()
    {
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        await FluentActions.Awaiting(() => InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithAsyncVoid), nameof(StepDefClassWithAsyncVoid.AsyncVoidStepDef)))
                           .Should()
                           .ThrowAsync<SpecFlowException>()
                           .WithMessage("*async void*");

        StepDefClassWithAsyncVoid.WasInvokedAsyncVoidStepDef.Should().BeFalse();
    }

    #endregion
    
    #region ExecutionContext / AsyncLocal<T> related tests

    public enum AsyncLocalType
    {
        Uninitialized,
        CtorInitialized,
        StaticCtorInitialized,
        Boxed
    }

    class StepDefClassWithAsyncLocal
    {
        private readonly AsyncLocal<string> UninitializedAsyncLocal = new();
        private readonly AsyncLocal<string> CtorInitializedAsyncLocal = new(ValueChangedHandler) { Value = "42" };
        private static readonly AsyncLocal<string> StaticCtorInitializedAsyncLocal = new() { Value = "42" };
        private readonly AsyncLocal<StrongBox<string>> BoxedAsyncLocal = new(ValueChangedHandlerBoxed) { Value = new StrongBox<string>("42") };

        private static void ValueChangedHandler(AsyncLocalValueChangedArgs<string> obj)
        {
            _testOutputHelperInstance?.WriteLine($"{obj.PreviousValue} -> {obj.CurrentValue} ThreadContextChanged: {obj.ThreadContextChanged}");
            //_testOutputHelperInstance?.WriteLine(Environment.StackTrace);
        }

        private static void ValueChangedHandlerBoxed(AsyncLocalValueChangedArgs<StrongBox<string>> obj)
        {
            //_testOutputHelperInstance.WriteLine($"{obj.PreviousValue} -> {obj.CurrentValue} ({obj.ThreadContextChanged})");
        }

        public string LoadedValue { get; set; }

        public void SetAsyncLocal_Sync(AsyncLocalType asyncLocalType)
        {
            switch (asyncLocalType)
            {
                case AsyncLocalType.Uninitialized:
                    UninitializedAsyncLocal.Value = "42";
                    break;
                case AsyncLocalType.CtorInitialized:
                    CtorInitializedAsyncLocal.Value = "42";
                    break;
                case AsyncLocalType.StaticCtorInitialized:
                    StaticCtorInitializedAsyncLocal.Value = "42";
                    break;
                case AsyncLocalType.Boxed:
                    BoxedAsyncLocal.Value!.Value = "42";
                    break;
            }
        }

        public async Task SetAsyncLocal_Async(AsyncLocalType asyncLocalType)
        {
            switch (asyncLocalType)
            {
                case AsyncLocalType.Uninitialized:
                    UninitializedAsyncLocal.Value = "42";
                    break;
                case AsyncLocalType.CtorInitialized:
                    CtorInitializedAsyncLocal.Value = "42";
                    break;
                case AsyncLocalType.StaticCtorInitialized:
                    StaticCtorInitializedAsyncLocal.Value = "42";
                    break;
                case AsyncLocalType.Boxed:
                    BoxedAsyncLocal.Value!.Value = "42";
                    break;
            }
            await Task.Delay(1);
        }

        public void GetAsyncLocal_Sync(AsyncLocalType asyncLocalType, string expectedResult)
        {
            LoadedValue = "GetAsyncLocal_Sync";
            switch (asyncLocalType)
            {
                case AsyncLocalType.Uninitialized:
                    LoadedValue = UninitializedAsyncLocal.Value;
                    break;
                case AsyncLocalType.CtorInitialized:
                    LoadedValue = CtorInitializedAsyncLocal.Value;
                    break;
                case AsyncLocalType.StaticCtorInitialized:
                    LoadedValue = StaticCtorInitializedAsyncLocal.Value;
                    break;
                case AsyncLocalType.Boxed:
                    LoadedValue = BoxedAsyncLocal.Value?.Value;
                    break;
            }
            Assert.Equal(expectedResult, LoadedValue);
        }

        public async Task GetAsyncLocal_Async(AsyncLocalType asyncLocalType, string expectedResult)
        {
            _testOutputHelperInstance.WriteLine("reading");
            LoadedValue = "GetAsyncLocal_Async";
            switch (asyncLocalType)
            {
                case AsyncLocalType.Uninitialized:
                    LoadedValue = UninitializedAsyncLocal.Value;
                    break;
                case AsyncLocalType.CtorInitialized:
                    LoadedValue = CtorInitializedAsyncLocal.Value;
                    break;
                case AsyncLocalType.StaticCtorInitialized:
                    LoadedValue = StaticCtorInitializedAsyncLocal.Value;
                    break;
                case AsyncLocalType.Boxed:
                    LoadedValue = BoxedAsyncLocal.Value?.Value;
                    break;
            }
            Assert.Equal(expectedResult, LoadedValue);
            await Task.Delay(1);
        }
    }


    [Fact]
    public async Task Test1()
    {
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        var al = AsyncLocalType.CtorInitialized;

        //contextManager.ScenarioContext.ScenarioContainer.Resolve<StepDefClass>();

        _testOutputHelper.WriteLine("M1");
        await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithAsyncLocal), nameof(StepDefClassWithAsyncLocal.SetAsyncLocal_Sync), al);
        _testOutputHelper.WriteLine("/M1");
        _testOutputHelper.WriteLine("M2");
        await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithAsyncLocal), nameof(StepDefClassWithAsyncLocal.GetAsyncLocal_Async), al, "42");
        _testOutputHelper.WriteLine("/M2");

        var stepDefClass = contextManager.ScenarioContext.ScenarioContainer.Resolve<StepDefClassWithAsyncLocal>();
        stepDefClass.LoadedValue.Should().Be("42");
    }

    [Theory]
    [InlineData("Case 1/a", AsyncLocalType.Uninitialized, "Sync", "Sync", "42")]
    [InlineData("Case 1/b", AsyncLocalType.Uninitialized, "Sync", "Async", "42")]
    [InlineData("Case 2/a", AsyncLocalType.CtorInitialized, null, "Sync", "42")]
    [InlineData("Case 2/b", AsyncLocalType.CtorInitialized, null, "Async", "42")]
    [InlineData("Case 2x/a", AsyncLocalType.CtorInitialized, "Sync", "Sync", "42")]
    [InlineData("Case 2x/b", AsyncLocalType.CtorInitialized, "Async", "Async", "42")]
    //[InlineData("Case 3/a", AsyncLocalType.StaticCtorInitialized, null, "Sync", "42")]
    //[InlineData("Case 3/b", AsyncLocalType.StaticCtorInitialized, null, "Async", "42")]
    [InlineData("Case 4/a", AsyncLocalType.Uninitialized, "Async", "Sync", null)]
    [InlineData("Case 4/b", AsyncLocalType.Uninitialized, "Async", "Async", null)]
    [InlineData("Case 6/a", AsyncLocalType.Boxed, "Sync", "Sync", "42")]
    [InlineData("Case 6/b", AsyncLocalType.Boxed, "Sync", "Async", "42")]
    [InlineData("Case 7/a", AsyncLocalType.Boxed, null, "Sync", "42")]
    [InlineData("Case 7/b", AsyncLocalType.Boxed, null, "Async", "42")]
    [InlineData("Case 8/a", AsyncLocalType.Boxed, "Async", "Sync", "42")]
    [InlineData("Case 8/b", AsyncLocalType.Boxed, "Async", "Async", "42")]
    public async Task ExecutionContext_is_flowing_down_correctly_to_step_definitions(string description, AsyncLocalType asyncLocalType, string setAs, string getAs, string expectedResult)
    {
        _testOutputHelper.WriteLine(description);
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        if (setAs != null)
            await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithAsyncLocal), "SetAsyncLocal_" + setAs, asyncLocalType);
        await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassWithAsyncLocal), "GetAsyncLocal_" + getAs, asyncLocalType, expectedResult);

        var stepDefClass = contextManager.ScenarioContext.ScenarioContainer.Resolve<StepDefClassWithAsyncLocal>();
        stepDefClass.LoadedValue.Should().Be(expectedResult, $"Error was not propagated for {description}");
    }

    #endregion   
}