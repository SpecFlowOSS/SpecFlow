using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class StronglyTypedContextAcessorTests
    {
        [Test]
        public void Can_set_object_according_to_generic_type()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(expected);

            var actual = scenarioContext[typeof (IScenarioTestInterface).ToString()];

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Can_get_object_according_to_generic_type()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();
            scenarioContext[typeof (IScenarioTestInterface).ToString()] = expected;

            var actual = scenarioContext.Get<IScenarioTestInterface>();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Can_set_object_according_to_generic_type_and_string()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(expected, "test");

            var actual = scenarioContext["test"];

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Can_get_object_according_to_generic_type_and_string()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();
            scenarioContext["test"] = expected;

            var actual = scenarioContext.Get<IScenarioTestInterface>("test");

            Assert.AreSame(expected, actual);
        }

        private static ScenarioContext CreateScenarioContext()
        {
            return new ScenarioContext(new ScenarioInfo("Test", new string[] {}));
        }

        public class ScenarioTestClass : IScenarioTestInterface
        {
        }

        public interface IScenarioTestInterface
        {
        }
    }
}