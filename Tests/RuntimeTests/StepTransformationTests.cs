using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;

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

        [SetUp]
        public void SetUp()
        {
            // ScenarioContext is needed, because the [Binding]-instances live there
            ObjectContainer.ScenarioContext = new ScenarioContext(null);
        }

        [Test]
        public void UserConverterShouldConvertStringToUser()
        {
            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            StepTransformationBinding stepTransformationBinding = new StepTransformationBinding(@"user (\w+)", transformMethod);

            Assert.True(stepTransformationBinding.Regex.IsMatch("user xyz"));

            var result = stepTransformationBinding.BindingAction.DynamicInvoke("xyz");
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseUserConverterForConversion()
        {
            ObjectContainer.ScenarioContext = new ScenarioContext(null);
            BindingRegistry bindingRegistry = new BindingRegistry();
            ObjectContainer.BindingRegistry = bindingRegistry;

            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("Create");
            bindingRegistry.StepTransformations.Add(new StepTransformationBinding(@"user (\w+)", transformMethod));

            var stepArgumentTypeConverter = new StepArgumentTypeConverter();

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US"));
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }

        [Test]
        public void ShouldUseStepArgumentTransformationToConvertTable()
        {
            ObjectContainer.ScenarioContext = new ScenarioContext(null);
            BindingRegistry bindingRegistry = new BindingRegistry();
            ObjectContainer.BindingRegistry = bindingRegistry;

            UserCreator stepTransformationInstance = new UserCreator();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("CreateUsers");
            bindingRegistry.StepTransformations.Add(new StepTransformationBinding(@"", transformMethod));

            var stepArgumentTypeConverter = new StepArgumentTypeConverter();

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