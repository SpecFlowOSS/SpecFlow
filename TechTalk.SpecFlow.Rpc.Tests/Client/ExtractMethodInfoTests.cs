using FluentAssertions;
using TechTalk.SpecFlow.Rpc.Client;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Client
{
    public class ExtractMethodInfoTests
    {
        public ExtractMethodInfoTests()
        {
            _extractMethodInfo = new ExtractMethodInfo<ITestInterface>();
        }

        private readonly ExtractMethodInfo<ITestInterface> _extractMethodInfo;

        interface ITestInterface
        {
            void MethodWithoutParameter();
            void MethodWithParameter(string parameter);
            int FunctionWithoutParameter();
            int FunctionWithParameter(string parameter);
        }

        class PropertyHolder
        {
            public string Property { get; set; }
        }

        private void AssertInterfaceMethodInfo(InterfaceMethodInfo interfaceMethodInfo, string expectedMethodName, string expectedParameter)
        {
            interfaceMethodInfo.Assembly.Should().StartWith("TechTalk.SpecFlow.Rpc.Tests");
            interfaceMethodInfo.Typename.Should().Be("TechTalk.SpecFlow.Rpc.Tests.Client.ExtractMethodInfoTests+ITestInterface");

            interfaceMethodInfo.Methodname.Should().Be(expectedMethodName);
            interfaceMethodInfo.Arguments.Count.Should().Be(1);
            interfaceMethodInfo.Arguments[0].Should().Be(expectedParameter);
        }

        private void AssertInterfaceMethodInfo(InterfaceMethodInfo interfaceMethodInfo, string expectedMethodName)
        {
            interfaceMethodInfo.Assembly.Should().StartWith("TechTalk.SpecFlow.Rpc.Tests");
            interfaceMethodInfo.Typename.Should().Be("TechTalk.SpecFlow.Rpc.Tests.Client.ExtractMethodInfoTests+ITestInterface");

            interfaceMethodInfo.Methodname.Should().Be(expectedMethodName);
            interfaceMethodInfo.Arguments.Count.Should().Be(0);
        }

        [Fact]
        public void ExtractMethod_ConstantArgument()
        {
            var interfaceMethodInfo = _extractMethodInfo.ExtractMethod(c => c.MethodWithParameter("constant"));

            AssertInterfaceMethodInfo(interfaceMethodInfo, "MethodWithParameter", "constant");
        }

        [Fact]
        public void ExtractMethod_PropertyArgument()
        {
            var x = new PropertyHolder()
            {
                Property = "propertyValue"
            };


            var interfaceMethodInfo = _extractMethodInfo.ExtractMethod(c => c.MethodWithParameter(x.Property));
            AssertInterfaceMethodInfo(interfaceMethodInfo, "MethodWithParameter", "propertyValue");
        }

        [Fact]
        public void ExtractMethod_VariableArgument()
        {
            string parameter = "local_variable";
            var interfaceMethodInfo = _extractMethodInfo.ExtractMethod(c => c.MethodWithParameter(parameter));
            AssertInterfaceMethodInfo(interfaceMethodInfo, "MethodWithParameter", "local_variable");
        }

        [Fact]
        public void ExtractFunction_ConstantArgument()
        {
            var interfaceFunctionInfo = _extractMethodInfo.ExtractFunction(c => c.FunctionWithParameter("constant"));

            AssertInterfaceMethodInfo(interfaceFunctionInfo, "FunctionWithParameter", "constant");
        }

        [Fact]
        public void ExtractFunction_PropertyArgument()
        {
            var x = new PropertyHolder()
            {
                Property = "propertyValue"
            };


            var interfaceFunctionInfo = _extractMethodInfo.ExtractFunction(c => c.FunctionWithParameter(x.Property));
            AssertInterfaceMethodInfo(interfaceFunctionInfo, "FunctionWithParameter", "propertyValue");
        }

        [Fact]
        public void ExtractFunction_VariableArgument()
        {
            string parameter = "local_variable";
            var interfaceFunctionInfo = _extractMethodInfo.ExtractFunction(c => c.FunctionWithParameter(parameter));
            AssertInterfaceMethodInfo(interfaceFunctionInfo, "FunctionWithParameter", "local_variable");
        }


        [Fact]
        public void ExtractFunction_NoArgument()
        {
            var interfaceFunctionInfo = _extractMethodInfo.ExtractFunction(c => c.FunctionWithoutParameter());

            AssertInterfaceMethodInfo(interfaceFunctionInfo, "FunctionWithoutParameter");
        }

        [Fact]
        public void ExtractMethod_NoArgument()
        {
            var interfaceFunctionInfo = _extractMethodInfo.ExtractMethod(c => c.MethodWithoutParameter());

            AssertInterfaceMethodInfo(interfaceFunctionInfo, "MethodWithoutParameter");
        }
    }
}