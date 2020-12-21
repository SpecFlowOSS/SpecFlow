using System;
using System.Globalization;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using ScenarioExecutionStatus = TechTalk.SpecFlow.ScenarioExecutionStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    internal static class LegacyStepArgumentTypeConverterExtensions
    {
        public static object Convert(this IStepArgumentTypeConverter converter, object value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            return converter.Convert(value, new RuntimeBindingType(typeToConvertTo), cultureInfo);
        }


        public static Expression<Func<IStepArgumentTypeConverter, bool>> GetCanConvertMethodFilter(object argument, Type type)
        {
            return c => c.CanConvert(argument, It.Is<IBindingType>(bt => bt.TypeEquals(type)), It.IsAny<CultureInfo>());
        }

        public static Expression<Func<IStepArgumentTypeConverter, object>> GetConvertMethodFilter(object argument, Type type)
        {
            return c => c.Convert(It.Is<object>(s => s.Equals(argument)), It.Is<IBindingType>(bt => bt.TypeEquals(type)), It.IsAny<CultureInfo>());
                //Arg<string>.Is.Equal(argument),
                //Arg<IBindingType>.Matches(bt => bt.TypeEquals(type)), 
                //Arg<CultureInfo>.Is.Anything);
        }     
    }

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

    
    public class StepExecutionTestsWithConversions : StepExecutionTestsBase
    {
        [Fact]
        public void ShouldCallBindingWithSimpleConvertParam()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //bindingInstance.Expect(b => b.BindingWithSimpleConvertParam(1.23));

            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step with simple convert param: 1.23");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithSimpleConvertParam(1.23));
        }

        [Fact]
        public void ShouldRaiseErrorIfSimpleConvertParamFails()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step with simple convert param: not-a-double");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.TestError);
        }

        [Fact]
        public void ShouldCallTheOnlyThatCanConvert()
        {
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter("argument", typeof(double))).Returns(true);
            //StepArgumentTypeConverterStub.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>())).Returns(false);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter("argument", typeof(double))).Returns(1.23);

            var (testRunner, bindingMock) = GetTestRunnerWithConverterStub<StepExecutionTestsBindingsForArgumentConvert>();

            


            //bindingInstance.Expect(b => b.DoubleArg(1.23));

            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert: argument");


            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK, ContextManagerStub.ScenarioContext.TestError?.ToString());
            bindingMock.Verify(x => x.DoubleArg(1.23));
            //StepArgumentTypeConverterStub.Verify(c => c.Convert("argument", It.Is<IBindingType>(bt => bt.TypeEquals(typeof(double))), It.IsAny<CultureInfo>())).; LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter("argument", typeof(double))).Return(1.23);
        }

       

        [Fact]
        public void ShouldRaiseAmbiguousIfMultipleCanConvert()
        {
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter("argument", typeof(double))).Returns(true);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter("argument", typeof(int))).Returns(true);
            StepArgumentTypeConverterStub.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>())).Returns(false);

            var (testRunner, bindingMock) = GetTestRunnerWithConverterStub<StepExecutionTestsBindingsForArgumentConvert>();

            // return false unless its a Double or an Int
            
            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert: argument");


            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.BindingError, ContextManagerStub.ScenarioContext.TestError?.ToString());
        }

        [Fact]
        public void ShouldCallTheOnlyThatCanConvertWithTable()
        {
            Table table = new Table("h1");

            // return false unless its a Double or table->table
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetCanConvertMethodFilter("argument", typeof(double))).Returns(true);
            //StepArgumentTypeConverterStub.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>())).Returns(false);

            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter(table, typeof(Table))).Returns(table);
            StepArgumentTypeConverterStub.Setup(LegacyStepArgumentTypeConverterExtensions.GetConvertMethodFilter("argument", typeof(double))).Returns(1.23);


            var (testRunner, bindingMock) = GetTestRunnerWithConverterStub<StepExecutionTestsBindingsForArgumentConvert>();

            
            //bindingInstance.Expect(b => b.DoubleArgWithTable(1.23, table));

            //MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert with table: argument", null, table);


            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK, ContextManagerStub.ScenarioContext.TestError?.ToString());
            bindingMock.Verify(x => x.DoubleArgWithTable(1.23, table));
        }

        [Fact]
        public void ShouldRaiseParamErrorIfNoneCanConvert()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindingsForArgumentConvert>();

            // none can convert
            StepArgumentTypeConverterStub.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>())).Returns(false);

           // MockRepository.ReplayAll();

            testRunner.GivenAsync("sample step for argument convert: argument");

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.BindingError, ContextManagerStub.ScenarioContext.TestError?.ToString());
        }
    }
}