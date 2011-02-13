using System.Globalization;
using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Bindings;

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

            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldRaiseErrorIfSimpleConvertParamFails()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            MockRepository.ReplayAll();

            testRunner.Given("sample step with simple convert param: not-a-double");

            Assert.AreEqual(TestStatus.TestError, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheOnlyThatCanConvert()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            // return false unless its a Double
            converter.Stub(c => c.CanConvert("argument", typeof(double), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            converter.Expect(c => c.Convert("argument", typeof(double), FeatureLanguage)).Return(1.23);
            bindingInstance.Expect(b => b.DoubleArg(1.23));

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");


            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldRaiseAmbiguousIfMultipleCanConvert()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            // return false unless its a Double or an Int
            converter.Stub(c => c.CanConvert("argument", typeof(double), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert("argument", typeof(int), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");


            Assert.AreEqual(TestStatus.BindingError, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheOnlyThatCanConvertWithTable()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");

            // return false unless its a Double or table->table
            converter.Stub(c => c.CanConvert("argument", typeof(double), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            converter.Expect(c => c.Convert("argument", typeof(double), FeatureLanguage)).Return(1.23);
            bindingInstance.Expect(b => b.DoubleArgWithTable(1.23, table));

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with table: argument", null, table);


            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldRaiseParamErrorIfNoneCanConvert()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            // none can convert
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");

            Assert.AreEqual(TestStatus.BindingError, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }



    }
}