using NUnit.Framework;
using Rhino.Mocks;

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
        public void SholdCallBindingWithSimpleConvertParam()
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
        public void SholdRaiseErrorIfSimpleConvertParamFails()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            MockRepository.ReplayAll();

            testRunner.Given("sample step with simple convert param: not-a-double");

            Assert.AreEqual(TestStatus.TestError, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallTheOnlyThatCanConvert()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            // everything else than double cannot convert is false
            converter.Stub(c => c.CanConvert("argument", typeof(double), Language)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            converter.Expect(c => c.Convert("argument", typeof(double), Language)).Return(1.23);
            bindingInstance.Expect(b => b.DoubleArg(1.23));

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");


            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdRaiseAmbiguousIfMultipleCanConvert()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            // everything else than double and int cannot convert is false
            converter.Stub(c => c.CanConvert("argument", typeof(double), Language)).Return(true);
            converter.Stub(c => c.CanConvert("argument", typeof(int), Language)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert: argument");


            Assert.AreEqual(TestStatus.BindingError, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallTheOnlyThatCanConvertWithTable()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;

            StepExecutionTestsBindingsForArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");

            // everything else than double cannot convert is false
            converter.Stub(c => c.CanConvert("argument", typeof(double), Language)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);

            converter.Expect(c => c.Convert("argument", typeof(double), Language)).Return(1.23);
            bindingInstance.Expect(b => b.DoubleArgWithTable(1.23, table));

            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with table: argument", null, table);


            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdRaiseParamErrorIfNoneCanConvert()
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