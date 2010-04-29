using System.Collections.Generic;
using System.Globalization;
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
        [StepTransformation("user (w+)")]
        public User Create(string name)
        {
            return new User {Name = name};
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
            StepTransformation stepTransformation = new StepTransformation(@"user (\w+)", transformMethod);

            Assert.True(stepTransformation.Regex.IsMatch("user xyz"));

            var result = stepTransformation.BindingAction.DynamicInvoke("xyz");
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
            bindingRegistry.StepTransformations.Add(new StepTransformation(@"user (\w+)", transformMethod));

            var stepArgumentTypeConverter = new StepArgumentTypeConverter();

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US"));
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }


    }

}