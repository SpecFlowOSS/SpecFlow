using NUnit.Framework;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using FluentAssertions;
using Moq;
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

        [Test]
        public void GetMethodText_should_return_string_containing_full_assembly_name__method_name_and_parameters_types()
        {
            var errorProvider = CreateErrorProvider();

            var bindingMethodTypeStub = new Mock<IBindingType>();
            bindingMethodTypeStub.Setup(t => t.FullName).Returns("StepsAssembly1.CalculatorSteps");

            var bindingMethodStub = new Mock<IBindingMethod>();
            bindingMethodStub.Setup(m => m.Type).Returns(bindingMethodTypeStub.Object);
            bindingMethodStub.Setup(m => m.Name).Returns("WhenIAdd");

            var bindingParameter1TypeStub = new Mock<IBindingType>();
            bindingParameter1TypeStub.Setup(t => t.Name).Returns("Int32");
            var bindingParameter1Stub = new Mock<IBindingParameter>();
            bindingParameter1Stub.Setup(p => p.Type).Returns(bindingParameter1TypeStub.Object);

            var bindingParameter2TypeStub = new Mock<IBindingType>();
            bindingParameter2TypeStub.Setup(t => t.Name).Returns("String");
            var bindingParameter2Stub = new Mock<IBindingParameter>();
            bindingParameter2Stub.Setup(p => p.Type).Returns(bindingParameter2TypeStub.Object);


            bindingMethodStub.Setup(m => m.Parameters).Returns(new[] { bindingParameter1Stub.Object, bindingParameter2Stub.Object });
            
            var result = errorProvider.GetMethodText(bindingMethodStub.Object);
            result.Should().NotBeNull();
            result.Should().Be("StepsAssembly1.CalculatorSteps.WhenIAdd(Int32, String)");
        }
    }
}