using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class ErrorProviderTests
    {
        private static ErrorProvider CreateErrorProvider(IStepFormatter stepFormatter = null, SpecFlow.Configuration.SpecFlowConfiguration specFlowConfiguration = null, IUnitTestRuntimeProvider unitTestRuntimeProvider = null)
        {
            return new ErrorProvider(stepFormatter, specFlowConfiguration, unitTestRuntimeProvider);
        }

        private IBindingMethod CreateBindingMethod(string methodName, string methodBindingTypeName, string methodBindingTypeFullName, params string[] parametersTypes)
        {
            parametersTypes = parametersTypes ?? new string[0];
            return new BindingMethod(
                new BindingType(methodBindingTypeName, methodBindingTypeFullName),
                methodName,
                parametersTypes.Select(pn => new BindingParameter(new BindingType(pn, pn),string.Empty)),
                null);
        }

        [Test]
        public void GetMethodText_should_return_string_containing_full_assembly_name_method_name_and_parameters_types()
        {
            const string methodName = "WhenIAdd";
            const string methodBindingTypeName = "CalculatorSteps";
            const string methodBindingTypeFullName = "StepsAssembly1.CalculatorSteps";
            const string parameter1Type = "Int32";
            const string parameter2Type = "String";

            var errorProvider = CreateErrorProvider();

            var bindingMethod = CreateBindingMethod(methodName, methodBindingTypeName, methodBindingTypeFullName, parameter1Type, parameter2Type);
            
            var result = errorProvider.GetMethodText(bindingMethod);

            result.Should().NotBeNull();
            result.Should().Be($"{methodBindingTypeFullName}.{ methodName}({ parameter1Type}, { parameter2Type})");
        }

        [Test]
        public void GetCallError_should_return_BindingException_containing_full_assembly_name_method_name_and_parameters_types_and_exception_message()
        {
            const string methodName = "WhenIMultiply";
            const string methodBindingTypeName = "CalculatorSteps";
            const string methodBindingTypeFullName = "StepsAssembly1.CalculatorSteps";
            const string parameter1Type = "String";
            const string parameter2Type = "Int64";

            const string expectedExceptionMessage = "Initialization failed";

            var errorProvider = CreateErrorProvider();

            var bindingMethod = CreateBindingMethod(methodName, methodBindingTypeName, methodBindingTypeFullName, parameter1Type, parameter2Type);

            var exceptionStub = new Mock<Exception>();
            exceptionStub.Setup(e => e.Message).Returns(expectedExceptionMessage);

            var result = errorProvider.GetCallError(bindingMethod, exceptionStub.Object);

            result.Should().NotBeNull();
            result.Should().BeOfType<BindingException>();
            result.Message.Should().Be($"Error calling binding method '{methodBindingTypeFullName}.{methodName}({parameter1Type}, {parameter2Type})': {expectedExceptionMessage}");
        }

        [Test]
        public void GetParameterCountError_should_return_BindingException_containing_full_assembly_name_method_name_and_parameters_types_and_expected_parameter_count()
        {
            const string methodName = "WhenIMultiply";
            const string methodBindingTypeName = "CalculatorSteps";
            const string methodBindingTypeFullName = "StepsAssembly1.CalculatorSteps";
            const string parameter1Type = "Int64";

            var errorProvider = CreateErrorProvider();

            var bindingMethod = CreateBindingMethod(methodName, methodBindingTypeName, methodBindingTypeFullName, parameter1Type);

            var stepDefinitionStub = new Mock<IStepDefinitionBinding>();
            stepDefinitionStub.Setup(sd => sd.Method).Returns(bindingMethod);
            var result = errorProvider.GetParameterCountError(new BindingMatch(stepDefinitionStub.Object, It.IsAny<int>(),null, null), 2);

            result.Should().NotBeNull();
            result.Should().BeOfType<BindingException>();
            result.Message.Should().Be($"Parameter count mismatch! The binding method '{methodBindingTypeFullName}.{methodName}({parameter1Type})' should have 2 parameters");
        }

        [Test]
        public void GetAmbiguousMatchError_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description()
        {
            const string prefixMessage = "Ambiguous step definitions found for step";
            GetMatchErrorMethod_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description(
                (errorProvider,matches, stepInstance) => errorProvider.GetAmbiguousMatchError(matches, stepInstance), prefixMessage);
        }

        [Test]
        public void GetAmbiguousBecauseParamCheckMatchError_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description()
        {
            const string prefixMessage = "Multiple step definitions found, but none of them have matching parameter count and type for step";
            GetMatchErrorMethod_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description(
                (errorProvider, matches, stepInstance) => errorProvider.GetAmbiguousBecauseParamCheckMatchError(matches, stepInstance), prefixMessage);
        }

        [Test]
        public void GetNoMatchBecauseOfScopeFilterError_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description()
        {
            const string prefixMessage = "Multiple step definitions found, but none of them have matching scope for step";
            GetMatchErrorMethod_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description(
                (errorProvider, matches, stepInstance) => errorProvider.GetNoMatchBecauseOfScopeFilterError(matches, stepInstance), prefixMessage);
        }

        private void GetMatchErrorMethod_should_return_BindingException_containing_full_assembly_names_method_names_parameters_types_and_step_description(Func<ErrorProvider, List<BindingMatch>, StepInstance, Exception>GetMatchErrorFunc,string expectedPrefixMessage)
        {
            const string methodName = "WhenIMultiply";
            const string methodBindingTypeName = "CalculatorSteps";
            const string method1BindingTypeFullName = "StepsAssembly1.CalculatorSteps";
            const string method2BindingTypeFullName = "StepsAssembly1.CalculatorSteps";
            const string parameter1Type = "Int64";

            const string stepInstanceDescription = "'Given I multiply 10 and 5'";

            var stepFormatterStub = new Mock<IStepFormatter>();
            stepFormatterStub.Setup(f => f.GetStepDescription(It.IsAny<StepInstance>())).Returns(stepInstanceDescription);
            var errorProvider = CreateErrorProvider(stepFormatterStub.Object);

            var bindingMethod1 = CreateBindingMethod(methodName, methodBindingTypeName, method1BindingTypeFullName, parameter1Type);
            var bindingMethod2 = CreateBindingMethod(methodName, methodBindingTypeName, method2BindingTypeFullName, parameter1Type);

            var stepDefinitionStub1 = new Mock<IStepDefinitionBinding>();
            stepDefinitionStub1.Setup(sd => sd.Method).Returns(bindingMethod1);
            var stepDefinitionStub2 = new Mock<IStepDefinitionBinding>();
            stepDefinitionStub2.Setup(sd => sd.Method).Returns(bindingMethod2);
            var bindingMatch = new List<BindingMatch>()
            {
                new BindingMatch(stepDefinitionStub1.Object, It.IsAny<int>(), null, null),
                new BindingMatch(stepDefinitionStub2.Object, It.IsAny<int>(), null, null)
            };
            var result = GetMatchErrorFunc(errorProvider,bindingMatch, null);

            result.Should().NotBeNull();
            result.Should().BeOfType<BindingException>();
            result.Message.Should().Be($"{expectedPrefixMessage} '{stepInstanceDescription}': {method1BindingTypeFullName}.{methodName}({parameter1Type}), {method2BindingTypeFullName}.{methodName}({parameter1Type})");
        }

    }
}