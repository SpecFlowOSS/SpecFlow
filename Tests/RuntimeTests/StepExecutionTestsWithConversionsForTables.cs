using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Bindings;

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
    public class StepExecutionTestsWithConversionsForTables : StepExecutionTestsBase
    {
        [Test]
        public void ShouldCallTheUserConverterToConvertTableWithTable()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;
            ObjectContainer.StepDefinitionSkeletonProvider(ProgrammingLanguage.CSharp);

            StepExecutionTestsBindingsForTableArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            var user = new User();

            // return false unless its a User
            converter.Stub(c => c.CanConvert(table, typeof(User), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);
            converter.Stub(c => c.Convert(table, typeof(User), FeatureLanguage)).Return(user);

            bindingInstance.Expect(b => b.SingleTable(user));
            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with table", null, table);

            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheUserConverterToConvertTableWithTableAndMultilineArg()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;
            ObjectContainer.StepDefinitionSkeletonProvider(ProgrammingLanguage.CSharp);

            StepExecutionTestsBindingsForTableArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            var user = new User();

            // return false unless its a User
            converter.Stub(c => c.CanConvert(table, typeof(User), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);
            converter.Stub(c => c.Convert(table, typeof(User), FeatureLanguage)).Return(user);

            var multiLineArg = "multi-line arg";
            bindingInstance.Expect(b => b.MultilineArgumentAndTable(multiLineArg, user));
            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with multiline argument and table", multiLineArg, table);

            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallTheUserConverterToConvertTableWithTableAndMultilineArgAndParameter()
        {
            var converter = MockRepository.Stub<IStepArgumentTypeConverter>();
            ObjectContainer.StepArgumentTypeConverter = converter;
            ObjectContainer.StepDefinitionSkeletonProvider(ProgrammingLanguage.CSharp);

            StepExecutionTestsBindingsForTableArgumentConvert bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            string argumentValue = "argument";
            var user = new User();

            // return false unless its a User
            // must also stub CanConvert & Convert for the string argument as we've introduced a parameter
            converter.Stub(c => c.CanConvert(table, typeof(User), FeatureLanguage)).Return(true);
            converter.Stub(c => c.CanConvert(null, null, null)).IgnoreArguments().Return(false);
            converter.Stub(c => c.Convert(table, typeof(User), FeatureLanguage)).Return(user);

            var multiLineArg = "multi-line arg";
            bindingInstance.Expect(b => b.ParameterMultilineArgumentAndTable(argumentValue, multiLineArg, user));
            MockRepository.ReplayAll();

            testRunner.Given("sample step for argument convert with parameter, multiline argument and table: argument", multiLineArg, table);

            Assert.AreEqual(TestStatus.OK, ObjectContainer.ScenarioContext.TestStatus);
            MockRepository.VerifyAll();
        }
    }
}