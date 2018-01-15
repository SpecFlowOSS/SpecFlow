using System;
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
        public void GetMethodText_should_return_string_containing_full_assembly_name__method_name_and_parameters_types()
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
        public void GetCallError_should_return_BindingException_containing_full_assembly_name__method_name_and_parameters_types_and_exception_message()
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
        public void GetParameterCountError_should_return_BindingException_containing_full_assembly_name__method_name_and_parameters_types_and_expected_parameter_count()
        {
            const string methodName = "WhenIMultiply";
            const string methodBindingTypeName = "CalculatorSteps";
            const string methodBindingTypeFullName = "StepsAssembly1.CalculatorSteps";
            const string parameter1Type = "Int64";

            const string expectedExceptionMessage = "Initialization failed";

            var errorProvider = CreateErrorProvider();

            var bindingMethod = CreateBindingMethod(methodName, methodBindingTypeName, methodBindingTypeFullName, parameter1Type);

            var exceptionStub = new Mock<Exception>();
            exceptionStub.Setup(e => e.Message).Returns(expectedExceptionMessage);

            var stepDefinitionStub = new Mock<IStepDefinitionBinding>();
            stepDefinitionStub.Setup(sd => sd.Method).Returns(bindingMethod);
            var result = errorProvider.GetParameterCountError(new BindingMatch(stepDefinitionStub.Object, It.IsAny<int>(),null, null), 2);

            result.Should().NotBeNull();
            result.Should().BeOfType<BindingException>();
            result.Message.Should().Be($"Parameter count mismatch! The binding method '{methodBindingTypeFullName}.{methodName}({parameter1Type})' should have 2 parameters");


        }

    }
}