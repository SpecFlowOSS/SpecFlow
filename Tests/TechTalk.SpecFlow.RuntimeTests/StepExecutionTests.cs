using System;
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

        [Given("^sample step with (single) param$")]
        public virtual void BindingWithSingleParam(string param)
        {
            
        }

        [Given("^sample step with (multi)(ple) param$")]
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

        [Given("^sample step with (mixed) params$")]
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
        public async Task ShouldCallBindingWithoutParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();
            
            //bindingInstance.Expect(b => b.BindingWithoutParam());

            await testRunner.GivenAsync("sample step without param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);

            bindingMock.Verify(x => x.BindingWithoutParam(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ShouldCallBindingSingleParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            await testRunner.GivenAsync("sample step with single param");
            
            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            
            bindingMock.Verify(x => x.BindingWithSingleParam("single"), Times.AtLeastOnce);
               
        }

        [Fact]
        public async Task ShouldCallBindingMultipleParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //bindingInstance.Expect(b => b.BindingWithMultipleParam("multi", "ple"));

            await testRunner.GivenAsync("sample step with multiple param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            
            bindingMock.Verify(x => x.BindingWithMultipleParam("multi", "ple"), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ShouldCallBindingWithTableParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            //bindingInstance.Expect(b => b.BindingWithTableParam(table));

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step with table param", null, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithTableParam(table));
        }

        [Fact]
        public async Task ShouldCallBindingWithMlStringParam()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            const string mlString = "ml-string";
            //bindingInstance.Expect(b => b.BindingWithMlStringParam(mlString));

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step with multi-line string param", mlString, null);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithMlStringParam(mlString));
        }

        [Fact]
        public async Task ShouldCallBindingWithTableAndMlStringParam()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            const string mlString = "ml-string";
            //bindingInstance.Expect(b => b.BindingWithTableAndMlStringParam(mlString, table));

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step with table and multi-line string param", mlString, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithTableAndMlStringParam(mlString, table));
        }

        [Fact]
        public async Task ShouldCallBindingWithMixedParams()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            const string mlString = "ml-string";
            //bindingInstance.Expect(b => b.BindingWithMixedParams("mixed", mlString, table));

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step with mixed params", mlString, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithMixedParams("mixed", mlString, table));
        }

        [Fact]
        public async Task ShouldRaiseAmbiguousIfMultipleMatch()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsAmbiguousBindings>();

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step without param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.BindingError);
        }

        [Fact]
        public async Task ShouldDistinguishByTableParam_CallWithoutTable()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //bindingInstance.Expect(b => b.DistinguishByTableParam1());

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("Distinguish by table param");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.DistinguishByTableParam1());
        }

        [Fact]
        public async Task ShouldDistinguishByTableParam_CallWithTable()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            Table table = new Table("h1");
            //bindingInstance.Expect(b => b.DistinguishByTableParam2(table));

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("Distinguish by table param", null, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.DistinguishByTableParam2(table));
        }

        [Fact]
        public async Task ShouldRaiseBindingErrorIfWrongParamNumber()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step with wrong param number");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.BindingError);
        }

        [Fact]
        public async Task ShouldCallBindingThatReturnsTask()
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

            await testRunner.GivenAsync("Returns a Task");
            Assert.True(taskFinished);

        }

        [Fact]
        public async Task ShouldCallBindingThatReturnsTaskAndReportError()
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

            await testRunner.GivenAsync("Returns a Task");
            Assert.True(taskFinished);
            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.TestError);
            Assert.Equal("catch meee", ContextManagerStub.ScenarioContext.TestError.Message);
            
        }
    }
}
