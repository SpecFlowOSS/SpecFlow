using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BoDi;
using FluentAssertions;
using Moq;
using Xunit;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class User
    {
        public string Name { get; set; }
    }

    [Binding]
    public class UserCreator
    {
        [StepArgumentTransformation("user (w+)")]
        public User Create(string name)
        {
            return new User {Name = name};
        }

        [StepArgumentTransformation]
        public IEnumerable<User> CreateUsers(Table table)
        {
            return table.Rows.Select(tableRow =>
                new User { Name = tableRow["Name"] });
        }
    }

    [Binding]
    public class TypeToTypeConverter
    {
        [StepArgumentTransformation("string (w+)")]
        public string StringToStringConvertRegex(string value)
        {
            return string.Concat("prefix ", value);
        }

        [StepArgumentTransformation]
        public string StringToStringConvert(string value)
        {
            return string.Concat("prefix ", value);
        }

        [StepArgumentTransformation]
        public Table TableToTableConvert(Table table)
        {
            var transformedTable = new List<string>();
            transformedTable.Add("transformed column");
            transformedTable.AddRange(table.Header);

            return new Table(transformedTable.ToArray());
        }
    }

    public class StepTransformationTests
    {
        private readonly Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
        private readonly Mock<IContextManager> contextManagerStub = new Mock<IContextManager>();
        private readonly Mock<IBindingInvoker> methodBindingInvokerStub = new Mock<IBindingInvoker>();
        private readonly List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();

        public StepTransformationTests()
        {
            // ScenarioContext is needed, because the [Binding]-instances live there
            var scenarioContext = new ScenarioContext(new ObjectContainer(), null, new TestObjectResolver());
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);

            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(string regexString, IBindingMethod transformMethod)
        {
            return new StepArgumentTransformationBinding(regexString, transformMethod);
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(string regexString, MethodInfo transformMethod)
        {
            return new StepArgumentTransformationBinding(regexString, new RuntimeBindingMethod(transformMethod));
        }

        [Fact]
        public void UserConverterShouldConvertStringToUser()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            var stepTransformationBinding = CreateStepTransformationBinding(@"user (\w+)", transformMethod);

            stepTransformationBinding.Regex.IsMatch("user xyz").Should().BeTrue();

            var invoker = new BindingInvoker(ConfigurationLoader.GetDefault(), new Mock<IErrorProvider>().Object, new SynchronousBindingDelegateInvoker());
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { "xyz" }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            result.Should().BeOfType<User>();
            ((User) result).Name.Should().Be("xyz");
        }

        [Fact]
        public void TypeToTypeConverterShouldConvertStringToStringUsingRegex()
        {
            TypeToTypeConverter stepTransformationInstance = new TypeToTypeConverter();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("StringToStringConvertRegex");
            var stepTransformationBinding = CreateStepTransformationBinding(@"string (\w+)", transformMethod);

            Assert.Matches(stepTransformationBinding.Regex, "string xyz");

            var invoker = new BindingInvoker(ConfigurationLoader.GetDefault(), new Mock<IErrorProvider>().Object, new SynchronousBindingDelegateInvoker());
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { "xyz" }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            result.GetType().Should().Be<string>();
            result.Should().Be("prefix xyz");
        }

        [Fact]
        public void TypeToTypeConverterShouldConvertStringToString()
        {
            TypeToTypeConverter stepTransformationInstance = new TypeToTypeConverter();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("StringToStringConvert");
            var stepTransformationBinding = CreateStepTransformationBinding(@"", transformMethod);

            var invoker = new BindingInvoker(ConfigurationLoader.GetDefault(), new Mock<IErrorProvider>().Object, new SynchronousBindingDelegateInvoker());
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { "xyz" }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            result.GetType().Should().Be<string>();
            result.Should().Be("prefix xyz");
        }

        [Fact]
        public void TypeToTypeConverterShouldConvertTableToTable()
        {
            TypeToTypeConverter stepTransformationInstance = new TypeToTypeConverter();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("TableToTableConvert");
            var stepTransformationBinding = CreateStepTransformationBinding(@"", transformMethod);

            var invoker = new BindingInvoker(ConfigurationLoader.GetDefault(), new Mock<IErrorProvider>().Object, new SynchronousBindingDelegateInvoker());
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { new Table("h1") }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);

            result.GetType().Should().Be<Table>();
            ((Table)result).Header.Should().BeEquivalentTo(new string[] { "transformed column", "h1" });
        }

        [Fact]
        public void StepArgumentTypeConverterShouldUseUserConverterForConversion()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = new RuntimeBindingMethod(stepTransformationInstance.GetType().GetMethod("Create"));
            var stepTransformationBinding = CreateStepTransformationBinding(@"user (\w+)", transformMethod);
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUser = new User();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US", false));
            result.Should().Be(resultUser);
        }

        private StepArgumentTypeConverter CreateStepArgumentTypeConverter()
        {
            return new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, contextManagerStub.Object, methodBindingInvokerStub.Object);
        }

        [Fact]
        public void ShouldUseStepArgumentTransformationToConvertTable()
        {
            var table = new Table("Name");
            
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = new RuntimeBindingMethod(stepTransformationInstance.GetType().GetMethod("CreateUsers"));
            var stepTransformationBinding = CreateStepTransformationBinding(@"", transformMethod);
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUsers = new User[3];
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), new object[] { table }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUsers);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();


            var result = stepArgumentTypeConverter.Convert(table, typeof(IEnumerable<User>), new CultureInfo("en-US", false));

            result.Should().NotBeNull();
            result.Should().Be(resultUsers);

        }
    }

}