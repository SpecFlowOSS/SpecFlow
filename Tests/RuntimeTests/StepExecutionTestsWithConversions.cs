using System.Globalization;
using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class StepExecutionTestsBindingsForArgumentConvert
    {
        [Given("sample step for argument convert: (.*)")]
        public virtual void IntArg(int param)
        {

        }

        [Given("sample step for argument convert: (.*)")]
        public virtual void DoubleArg(double param)
        {

        }

        [Given("sample step for argument convert with table: (.*)")]
        public virtual void IntArgWithTable(int param, Table table)
        {

        }

        [Given("sample step for argument convert with table: (.*)")]
        public virtual void DoubleArgWithTable(double param, Table table)
        {

        }
    }

    [TestFixture]
    public class StepExecutionTestsWithConversions : StepExecutionTestsBase
    {
        [Test]
        public void ShouldCallBindingWithSimpleConvertParam()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithSimpleConvertParam(1.23));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with simple convert param: 1.23");

            Assert.AreEqual(TestStatus.OK, ScenarioContext.Current.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldRaiseErrorIfSimpleConvertParamFails()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            MockRepository.ReplayAll();

            testRunner.Given("sample step with simple convert param: not-a-double");

            Assert.AreEqual(TestStatus.TestError, ScenarioContext.Current.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheOnlyThatCanConvert()
        {
            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            // return false unless its a Double
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert("argument", typeof(double), FeatureLanguage)).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            StepArgumentTypeConverterStub.Expect(c => c.Convert("argument", typeof(double), FeatureLanguage)).Return(1.23);
            bindingInstance.Expect(b => b.DoubleArg(1.23));

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");


            Assert.AreEqual(TestStatus.OK, ScenarioContext.Current.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldRaiseAmbiguousIfMultipleCanConvert()
        {
            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            // return false unless its a Double or an Int
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert("argument", typeof(double), FeatureLanguage)).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert("argument", typeof(int), FeatureLanguage)).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");


            Assert.AreEqual(TestStatus.BindingError, ScenarioContext.Current.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheOnlyThatCanConvertWithTable()
        {
            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            Table table = new Table("h1");

            // return false unless its a Double or table->table
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert("argument", typeof(double), FeatureLanguage)).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            StepArgumentTypeConverterStub.Expect(c => c.Convert("argument", typeof(double), FeatureLanguage)).Return(1.23);
            bindingInstance.Expect(b => b.DoubleArgWithTable(1.23, table));

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with table: argument", null, table);


            Assert.AreEqual(TestStatus.OK, ScenarioContext.Current.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldRaiseParamErrorIfNoneCanConvert()
        {
            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            // none can convert
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");

            Assert.AreEqual(TestStatus.BindingError, ScenarioContext.Current.TestStatus);
            MockRepository.VerifyAll();
        }



    }
}