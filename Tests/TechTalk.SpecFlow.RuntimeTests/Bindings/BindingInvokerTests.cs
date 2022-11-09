using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

    #region Exception Handling related tests - regression tests for SF2649
    public enum ExceptionKind
    {
        Normal,
        Aggregate
    }

    public enum InnerExceptionContentKind
    {
        WithInnerException,
        WithoutInnerException
    }

    public enum StepDefInvocationStyle
    {
        Sync,
        Async
    }

    class StepDefClassThatThrowsExceptions
    {
        public async Task AsyncThrow(ExceptionKind kindOfExceptionToThrow, InnerExceptionContentKind innerExceptionKind)
        {
            await Task.Run(() => ConstructAndThrowSync(kindOfExceptionToThrow, innerExceptionKind));
        }

        public void SyncThrow(ExceptionKind kindOfExceptionToThrow, InnerExceptionContentKind innerExceptionKind)
        {
            ConstructAndThrowSync(kindOfExceptionToThrow, innerExceptionKind);
        }

        private void ConstructAndThrowSync(ExceptionKind typeOfExceptionToThrow, InnerExceptionContentKind innerExceptionContentKind)
        {
            switch (typeOfExceptionToThrow, innerExceptionContentKind)
            {
                case (ExceptionKind.Normal, InnerExceptionContentKind.WithoutInnerException):
                    throw new Exception("Normal Exception message (No InnerException expected).");

                case (ExceptionKind.Aggregate, InnerExceptionContentKind.WithoutInnerException):
                    throw new AggregateException("AggregateEx (without Inners)");

                case (ExceptionKind.Normal, InnerExceptionContentKind.WithInnerException):
                    try
                    {
                        throw new Exception("This is the message from the Inner Exception");
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Normal Exception (with InnerException)", e);
                    }

                case (ExceptionKind.Aggregate, InnerExceptionContentKind.WithInnerException):
                    {
                        var tasks = new List<Task>
                        {
                            Task.Run(async () => throw new Exception("This is the first Exception embedded in the AggregateException")),
                            Task.Run(async () => throw new Exception("This is the second Exception embedded in the AggregateException"))
                        };
                        var continuation = Task.WhenAll(tasks);
                        
                        // This will throw an AggregateException with two Inner Exceptions
                        continuation.Wait();
                        return;
                    }
             }

        }
    }

    [Theory]
    [InlineData(StepDefInvocationStyle.Sync, InnerExceptionContentKind.WithoutInnerException)]
    [InlineData(StepDefInvocationStyle.Sync, InnerExceptionContentKind.WithInnerException)]
    [InlineData(StepDefInvocationStyle.Async, InnerExceptionContentKind.WithoutInnerException)]
    [InlineData(StepDefInvocationStyle.Async, InnerExceptionContentKind.WithInnerException)]
    public async Task InvokeBindingAsync_WhenStepDefThrowsExceptions_ProperlyPreservesExceptionContext(StepDefInvocationStyle style, InnerExceptionContentKind inner)
    {
        _testOutputHelper.WriteLine($"starting Exception Handling test: {style}, {inner}");
        var sut = CreateSut();
        var contextManager = CreateContextManagerWith();

        string methodToInvoke;
        ExceptionKind kindOfExceptionToThrow;
        Exception thrown;

        // call step definition methods
        if (style == StepDefInvocationStyle.Sync)
        {
            methodToInvoke = nameof(StepDefClassThatThrowsExceptions.SyncThrow);
            kindOfExceptionToThrow = ExceptionKind.Normal;
            thrown = await Assert.ThrowsAsync<Exception>(async () => await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassThatThrowsExceptions), methodToInvoke, kindOfExceptionToThrow, inner));
        }
        else // if (style == StepDefInvocationStyle.Async)
        {
            methodToInvoke = nameof(StepDefClassThatThrowsExceptions.AsyncThrow);
            kindOfExceptionToThrow = ExceptionKind.Aggregate;
            thrown = await Assert.ThrowsAsync<AggregateException>(async () => await InvokeBindingAsync(sut, contextManager, typeof(StepDefClassThatThrowsExceptions), methodToInvoke, kindOfExceptionToThrow, inner));
        }

        _testOutputHelper.WriteLine($"Exception detail: {thrown}");

        // Assert that the InnerException detail is preserved
        if (inner == InnerExceptionContentKind.WithInnerException)
        {
            if (thrown is AggregateException) (thrown as AggregateException).InnerExceptions.Count.Should().BeGreaterThan(1);
            else thrown.InnerException.Should().NotBeNull();
        }

        // Assert that the stack trace properly shows that the exception came from the throwing method (and not hidden by the SpecFlow infrastructure)
        thrown.StackTrace.Should().Contain(methodToInvoke);
    }
    #endregion

}