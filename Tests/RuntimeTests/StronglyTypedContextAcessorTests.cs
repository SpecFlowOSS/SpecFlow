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
        public void Can_set_object_with_a_generic_factory_method()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(() => expected);

            var actual = scenarioContext.Get<IScenarioTestInterface>();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Will_not_call_the_generic_factory_method_if_the_value_is_never_pulled_using_generic_Get()
        {
            var scenarioContext = CreateScenarioContext();

            var wasCalled = false;
            scenarioContext.Set<IScenarioTestInterface>(() =>
                                                            {
                                                                wasCalled = true;
                                                                return new ScenarioTestClass();
                                                            });

            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void Will_call_the_generic_factory_method_every_time_the_generic_get_is_called()
        {
            var scenarioContext = CreateScenarioContext();

            var numberOfTimesItTheMethodWasCalled = 0;
            scenarioContext.Set<IScenarioTestInterface>(() =>
            {
                numberOfTimesItTheMethodWasCalled++;
                return new ScenarioTestClass();
            });

            // call the get method twice
            scenarioContext.Get<IScenarioTestInterface>();
            scenarioContext.Get<IScenarioTestInterface>();

            Assert.AreEqual(2, numberOfTimesItTheMethodWasCalled);
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