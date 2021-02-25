﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;


namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class StepExecutionTestsBindings
    {
        [Given("sample step without param")]
        public virtual void BindingWithoutParam()
        {
            
        }

        [Given("sample step with (single) param")]
        public virtual void BindingWithSingleParam(string param)
        {
            
        }

        [Given("sample step with (multi)(ple) param")]
        public virtual void BindingWithMultipleParam(string param1, string param2)
        {
            
        }

        [Given("sample step with table param")]
        public virtual void BindingWithTableParam(Table table)
        {

        }

        [Given("sample step with multi-line string param")]
        public virtual void BindingWithMlStringParam(string multiLineString)
        {

        }

        [Given("sample step with table and multi-line string param")]
        public virtual void BindingWithTableAndMlStringParam(string multiLineString, Table table)
        {

        }

        [Given("sample step with (mixed) params")]
        public virtual void BindingWithMixedParams(string param1, string multiLineString, Table table)
        {

        }

        [Given("sample step with simple convert param: (.*)")]
        public virtual void BindingWithSimpleConvertParam(double param)
        {

        }

        [Given("sample step with wrong param number")]
        public virtual void BindingWithWrongParamNumber(double param)
        {

        }

        [Given("Distinguish by table param")]
        public virtual void DistinguishByTableParam1()
        {

        }

        [Given("Distinguish by table param")]
        public virtual void DistinguishByTableParam2(Table table)
        {

        }

        [Given("Returns a Task")]
        public virtual Task ReturnsATask()
        {
            throw new NotSupportedException("should be mocked");
        }
    }

    [Binding]
    public class StepExecutionTestsAmbiguousBindings
    {
        [Given("sample step without param")]
        public virtual void BindingWithoutParam1()
        {

        }

        [Given("sample step without param")]
        public virtual void BindingWithoutParam2()
        {

        }
    }

    
    public class StepExecutionTests : StepExecutionTestsBase
    {
        [Fact]
        public void ShouldCallBindingWithoutParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();
            
            //bindingInstance.Expect(b => b.BindingWithoutParam());

            testRunner.Given("sample step without param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);

            bindingMock.Verify(x => x.BindingWithoutParam(), Times.AtLeastOnce);
        }

        [Fact]
        public void ShouldCallBindingSingleParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            testRunner.Given("sample step with single param");
            
            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            
            bindingMock.Verify(x => x.BindingWithSingleParam("single"), Times.AtLeastOnce);
               
        }

        [Fact]
        public void ShouldCallBindingMultipleParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //bindingInstance.Expect(b => b.BindingWithMultipleParam("multi", "ple"));

            testRunner.Given("sample step with multiple param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            
            bindingMock.Verify(x => x.BindingWithMultipleParam("multi", "ple"), Times.AtLeastOnce);
        }

        [Fact]
        public void ShouldCallBindingWithTableParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            //bindingInstance.Expect(b => b.BindingWithTableParam(table));

            //MockRepository.ReplayAll();

            testRunner.Given("sample step with table param", null, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithTableParam(table));
        }

        [Fact]
        public void ShouldCallBindingWithMlStringParam()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            const string mlString = "ml-string";
            //bindingInstance.Expect(b => b.BindingWithMlStringParam(mlString));

            //MockRepository.ReplayAll();

            testRunner.Given("sample step with multi-line string param", mlString, null);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithMlStringParam(mlString));
        }

        [Fact]
        public void ShouldCallBindingWithTableAndMlStringParam()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            const string mlString = "ml-string";
            //bindingInstance.Expect(b => b.BindingWithTableAndMlStringParam(mlString, table));

            //MockRepository.ReplayAll();

            testRunner.Given("sample step with table and multi-line string param", mlString, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithTableAndMlStringParam(mlString, table));
        }

        [Fact]
        public void ShouldCallBindingWithMixedParams()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            const string mlString = "ml-string";
            //bindingInstance.Expect(b => b.BindingWithMixedParams("mixed", mlString, table));

            //MockRepository.ReplayAll();

            testRunner.Given("sample step with mixed params", mlString, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithMixedParams("mixed", mlString, table));
        }

        [Fact]
        public void ShouldRaiseAmbiguousIfMultipleMatch()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsAmbiguousBindings>();

            //MockRepository.ReplayAll();

            testRunner.Given("sample step without param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.BindingError);
        }

        [Fact]
        public void ShouldDistinguishByTableParam_CallWithoutTable()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //bindingInstance.Expect(b => b.DistinguishByTableParam1());

            //MockRepository.ReplayAll();

            testRunner.Given("Distinguish by table param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.DistinguishByTableParam1());
        }

        [Fact]
        public void ShouldDistinguishByTableParam_CallWithTable()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            //bindingInstance.Expect(b => b.DistinguishByTableParam2(table));

            //MockRepository.ReplayAll();

            testRunner.Given("Distinguish by table param", null, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.DistinguishByTableParam2(table));
        }

        [Fact]
        public void ShouldRaiseBindingErrorIfWrongParamNumber()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //MockRepository.ReplayAll();

            testRunner.Given("sample step with wrong param number");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.BindingError);
        }

        [Fact]
        public void ShouldCallBindingThatReturnsTask()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            bool taskFinished = false;

            bindingMock.Setup(m => m.ReturnsATask()).Returns(Task.Factory.StartNew(() =>
            {
                Thread.Sleep(800);
                taskFinished = true;
            }));

            //bindingInstance.Expect(b => b.ReturnsATask()).Return(Task.Factory.StartNew(() =>
            //    {
            //        Thread.Sleep(800);
            //        taskFinished = true;
            //    }));

            //MockRepository.ReplayAll();

            testRunner.Given("Returns a Task");
            Assert.True(taskFinished);

        }

        [Fact]
        public void ShouldCallBindingThatReturnsTaskAndReportError()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            bool taskFinished = false;
            bindingMock.Setup(m => m.ReturnsATask()).Returns(Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(800);
                        taskFinished = true;
                        throw new Exception("catch meee");
                    }));

            //bindingInstance.Expect(b => b.ReturnsATask()).Return(Task.Factory.StartNew(() =>
            //    {
            //        Thread.Sleep(800);
            //        taskFinished = true;
            //        throw new Exception("catch meee");
            //    }));

            //MockRepository.ReplayAll();

            testRunner.Given("Returns a Task");
            Assert.True(taskFinished);
            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.TestError);
            Assert.Equal("catch meee", ContextManagerStub.ScenarioContext.TestError.Message);
        }

        [Fact]
        public void ShouldCallAsyncBindingThatReturnsTaskAndReportErrorArgumentException()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            bool taskFinished = false;
            bindingMock.Setup(m => m.ReturnsATask()).Returns(Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(800);
                        taskFinished = true;
                        throw new ArgumentException("catch meee");
                    }));

            testRunner.Given("Returns a Task");
            Assert.True(taskFinished);
            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.TestError);
            Assert.Equal(typeof(ArgumentException), ContextManagerStub.ScenarioContext.TestError.GetType());
            Assert.Equal("catch meee", ContextManagerStub.ScenarioContext.TestError.Message);
        }

        [Fact]
        public void ShouldCallSyncBindingAndReportErrorArgumentException()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            bindingMock.Setup(m => m.BindingWithoutParam())
                .Throws(new ArgumentException("catch meee"));

            testRunner.Given("sample step without param");
            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.TestError);
            Assert.Equal(typeof(ArgumentException), ContextManagerStub.ScenarioContext.TestError.GetType());
            Assert.Equal("catch meee", ContextManagerStub.ScenarioContext.TestError.Message);
        }
    }
}
