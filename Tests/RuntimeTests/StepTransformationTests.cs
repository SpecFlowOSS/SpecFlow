using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
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
        [StepArgumentTransformation(@"user (\w+)")]
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

    [TestFixture]
    public class StepTransformationTests
    {
        private readonly Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
        private readonly Mock<IContextManager> contextManagerStub = new Mock<IContextManager>();
        private readonly List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();

        [SetUp]
        public void SetUp()
        {
            // ScenarioContext is needed, because the [Binding]-instances live there
            var scenarioContext = new ScenarioContext(null, null, null);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);

            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(MethodInfo transformMethod)
        {
            var regexString = transformMethod.GetCustomAttributes(typeof(StepArgumentTransformationAttribute), false)
                .OfType<StepArgumentTransformationAttribute>().Select(x => x.Regex).FirstOrDefault();
            return new StepArgumentTransformationBinding(regexString, new RuntimeBindingMethod(transformMethod));
        }

        [Test]
        public void UserConverterShouldConvertStringToUser()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            var stepTransformationBinding = CreateStepTransformationBinding(transformMethod);

            Assert.True(stepTransformationBinding.Regex.IsMatch("user xyz"));

            var invoker = new BindingInvoker(new RuntimeConfiguration(), new Mock<IErrorProvider>().Object);
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { "xyz" }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseUserConverterForConversion()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            var stepTransformationBinding = CreateStepTransformationBinding(transformMethod);
            stepTransformations.Clear();
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUser = new User();
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US"));
            Assert.That(result, Is.EqualTo(resultUser));
        }
        
        [Test]
        public void StepArgumentTypeConverterShouldNotUseUserConverterForStringConversionIfRegexDoesNotMatch()
        {
            var transformMethod = typeof(UserCreator).GetMethod("Create");
            var stepTransformationBinding = CreateStepTransformationBinding(transformMethod);
            stepTransformations.Clear();
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUser = new User();
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var result = stepArgumentTypeConverter.CanConvert("xyz", new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseUserConverterForNonStringConvertion()
        {
            var transformMethod = typeof(UserCreator).GetMethod("Create");
            var stepTransformationBinding = CreateStepTransformationBinding(transformMethod);
            stepTransformations.Clear();
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUser = new User();
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var table = new Table("Name");
            table.AddRow("xyz");
            var result = stepArgumentTypeConverter.Convert(table, new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.Not.EqualTo(resultUser));
        }

        private StepArgumentTypeConverter CreateStepArgumentTypeConverter(IBindingInvoker bindingInvoker)
        {
            return new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, contextManagerStub.Object, bindingInvoker);
        }

        [Test]
        public void ShouldUseStepArgumentTransformationToConvertTable()
        {
            var table = new Table("Name");
            
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("CreateUsers");
            var stepTransformationBinding = CreateStepTransformationBinding(transformMethod);
            stepTransformations.Clear();
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUsers = new User[3];
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), new object[] { table }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUsers);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);
            var result = stepArgumentTypeConverter.Convert(table, typeof(IEnumerable<User>), new CultureInfo("en-US"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(resultUsers));
        }
    }
}