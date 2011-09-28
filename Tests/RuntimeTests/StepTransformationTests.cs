using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
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

    [TestFixture]
    public class StepTransformationTests
    {
        private readonly Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
        private readonly Mock<IContextManager> contextManagerStub = new Mock<IContextManager>();

        [SetUp]
        public void SetUp()
        {
            // ScenarioContext is needed, because the [Binding]-instances live there
            var scenarioContext = new ScenarioContext(null, null, null);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);

            List<StepTransformationBinding> stepTransformations = new List<StepTransformationBinding>();
            bindingRegistryStub.Setup(br => br.StepTransformations).Returns(stepTransformations);
        }

        private StepTransformationBinding CreateStepTransformationBinding(string regexString, MethodInfo transformMethod)
        {
            return new StepTransformationBinding(new RuntimeConfiguration(), new Mock<IErrorProvider>().Object, regexString, transformMethod);
        }

        [Test]
        public void UserConverterShouldConvertStringToUser()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            StepTransformationBinding stepTransformationBinding = CreateStepTransformationBinding(@"user (\w+)", transformMethod);

            Assert.True(stepTransformationBinding.Regex.IsMatch("user xyz"));

            var result = stepTransformationBinding.BindingAction.DynamicInvoke(contextManagerStub.Object, "xyz");
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseUserConverterForConversion()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            bindingRegistryStub.Object.StepTransformations.Add(CreateStepTransformationBinding(@"user (\w+)", transformMethod));

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US"));
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }

        private StepArgumentTypeConverter CreateStepArgumentTypeConverter()
        {
            return new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, contextManagerStub.Object);
        }

        [Test]
        public void ShouldUseStepArgumentTransformationToConvertTable()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("CreateUsers");
            bindingRegistryStub.Object.StepTransformations.Add(CreateStepTransformationBinding(@"", transformMethod));

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();

            var table = new Table("Name");
            table.AddRow("Tom");
            table.AddRow("Dick");
            table.AddRow("Harry");

            var result = stepArgumentTypeConverter.Convert(table, typeof(IEnumerable<User>), new CultureInfo("en-US"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<User>>());
            Assert.That(((IEnumerable<User>)result).Count(), Is.EqualTo(3));
        }


    }

}