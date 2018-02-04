using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using ScenarioExecutionStatus = TechTalk.SpecFlow.ScenarioExecutionStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class StepExecutionTestsBindingsForTableArgumentConvert
    {

        [Given("sample step for argument convert with table")]
        public virtual void SingleTable(User table)
        {

        }


        [Given("sample step for argument convert with multiline argument and table")]
        public virtual void MultilineArgumentAndTable(string multilineArg, User table)
        {

        }

        [Given("sample step for argument convert with parameter, multiline argument and table: (.*)")]
        public virtual void ParameterMultilineArgumentAndTable(string param, string multilineArg, User table)
        {

        }
    }

    [TestFixture]
    public class StepExecutionTestsWithConversionsForTables : StepExecutionTestsBase
    {
        [Test]
        public void ShouldCallTheUserConverterToConvertTableWithTable()
        {
            StepExecutionTestsBindingsForTableArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            Table table = new Table("h1");
            var user = new User();

            // return false unless its a User
            StepArgumentTypeConverterStub.Stub(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter(table, typeof(User))).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);
            StepArgumentTypeConverterStub.Stub(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(User))).Return(user);

            bindingInstance.Expect(b => b.SingleTable(user));
            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with table", null, table);

            Assert.AreEqual(ScenarioExecutionStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheUserConverterToConvertTableWithTableAndMultilineArg()
        {
            StepExecutionTestsBindingsForTableArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            Table table = new Table("h1");
            var user = new User();

            // return false unless its a User
            StepArgumentTypeConverterStub.Stub(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter(table, typeof(User))).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);
            StepArgumentTypeConverterStub.Stub(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(User))).Return(user);

            var multiLineArg = "multi-line arg";
            bindingInstance.Expect(b => b.MultilineArgumentAndTable(multiLineArg, user));
            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with multiline argument and table", multiLineArg, table);

            Assert.AreEqual(ScenarioExecutionStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheUserConverterToConvertTableWithTableAndMultilineArgAndParameter()
        {
            StepExecutionTestsBindingsForTableArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerWithConverterStub(out bindingInstance);

            Table table = new Table("h1");
            string argumentValue = "argument";
            var user = new User();

            // return false unless its a User
            // must also stub CanConvert & Convert for the string argument as we've introduced a parameter
            StepArgumentTypeConverterStub.Stub(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter(table, typeof(User))).Return(true);
            StepArgumentTypeConverterStub.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);
            StepArgumentTypeConverterStub.Stub(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(User))).Return(user);

            var multiLineArg = "multi-line arg";
            bindingInstance.Expect(b => b.ParameterMultilineArgumentAndTable(argumentValue, multiLineArg, user));
            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with parameter, multiline argument and table: argument", multiLineArg, table);

            Assert.AreEqual(ScenarioExecutionStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }
    }
}