using System.Globalization;
using FluentAssertions;
using Moq;
using Xunit;
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

    
    public class StepExecutionTestsWithConversionsForTables : StepExecutionTestsBase
    {
        [Fact]
        public void ShouldCallTheUserConverterToConvertTableWithTable()
        {
            var (testRunner, bindingMock) = GetTestRunnerWithConverterStub<StepExecutionTestsBindingsForTableArgumentConvert>();

            Table table = new Table("h1");
            var user = new User();

            // return false unless its a User
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter(table, typeof(User))).Returns(true);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(User))).Returns(user);

            //bindingInstance.Expect(b => b.SingleTable(user));
            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert with table", null, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.SingleTable(user));
        }

        [Fact]
        public void ShouldCallTheUserConverterToConvertTableWithTableAndMultilineArg()
        {
            var (testRunner, bindingMock) = GetTestRunnerWithConverterStub<StepExecutionTestsBindingsForTableArgumentConvert>();

            Table table = new Table("h1");
            var user = new User();
            var multiLineArg = "multi-line arg";
            // return false unless its a User
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter(table, typeof(User))).Returns(true);            
            StepArgumentTypeConverterStub.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>())).Returns(false);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(multiLineArg, typeof(string))).Returns(multiLineArg);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(User))).Returns(user);
            

            
            //bindingInstance.Expect(b => b.MultilineArgumentAndTable(multiLineArg, user));
            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert with multiline argument and table", multiLineArg, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.MultilineArgumentAndTable(multiLineArg, user));
        }

        [Fact]
        public void ShouldCallTheUserConverterToConvertTableWithTableAndMultilineArgAndParameter()
        {
            var (testRunner, bindingMock) = GetTestRunnerWithConverterStub<StepExecutionTestsBindingsForTableArgumentConvert>();

            Table table = new Table("h1");
            string argumentValue = "argument";
            var user = new User();
            var multiLineArg = "multi-line arg";
            // return false unless its a User
            // must also stub CanConvert & Convert for the string argument as we've introduced a parameter
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter(table, typeof(User))).Returns(true);
            StepArgumentTypeConverterStub.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>())).Returns(false);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(User))).Returns(user);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(argumentValue, typeof(string))).Returns(argumentValue);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(multiLineArg, typeof(string))).Returns(multiLineArg);

            
            //bindingInstance.Expect(b => b.ParameterMultilineArgumentAndTable(argumentValue, multiLineArg, user));
            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert with parameter, multiline argument and table: argument", multiLineArg, table);

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.ParameterMultilineArgumentAndTable(argumentValue, multiLineArg, user));
        }
    }
}