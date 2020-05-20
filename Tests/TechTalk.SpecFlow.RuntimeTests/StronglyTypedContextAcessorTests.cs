using BoDi;
using Xunit;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class StronglyTypedContextAcessorTests
    {
        [Fact]
        public void Can_set_object_according_to_generic_type()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(expected);

            var actual = scenarioContext[typeof (IScenarioTestInterface).ToString()];

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Can_set_bool_with_to_generic_type()
        {
            var scenarioContext = CreateScenarioContext();

            scenarioContext.Set(true);
            Assert.True((bool) scenarioContext[typeof (bool).ToString()]);

            scenarioContext.Set(false);
            Assert.False((bool) scenarioContext[typeof (bool).ToString()]);
        }

        [Fact]
        public void Can_set_object_with_a_generic_factory_method()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(() => expected);

            var actual = scenarioContext.Get<IScenarioTestInterface>();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Will_not_call_the_generic_factory_method_if_the_value_is_never_pulled_using_generic_Get()
        {
            var scenarioContext = CreateScenarioContext();

            var wasCalled = false;
            scenarioContext.Set<IScenarioTestInterface>(() =>
                                                            {
                                                                wasCalled = true;
                                                                return new ScenarioTestClass();
                                                            });

            Assert.False(wasCalled);
        }

        [Fact]
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

            Assert.Equal(2, numberOfTimesItTheMethodWasCalled);
        }

        [Fact]
        public void Can_get_object_according_to_generic_type()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();
            scenarioContext[typeof (IScenarioTestInterface).ToString()] = expected;

            var actual = scenarioContext.Get<IScenarioTestInterface>();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Can_get_bool_with_generic_type()
        {
            var scenarioContext = CreateScenarioContext();

            scenarioContext[typeof (bool).ToString()] = true;
            Assert.True(scenarioContext.Get<bool>());

            scenarioContext[typeof (bool).ToString()] = false;
            Assert.False(scenarioContext.Get<bool>());
        }

        [Fact]
        public void Can_set_object_according_to_generic_type_and_string()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(expected, "test");

            var actual = scenarioContext["test"];

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Can_get_object_according_to_generic_type_and_string()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();
            scenarioContext["test"] = expected;

            var actual = scenarioContext.Get<IScenarioTestInterface>("test");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Can_retrieve_existing_value_with_try_get_value()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set(expected);

            ScenarioTestClass actual;
            var retrieved = scenarioContext.TryGetValue(out actual);

            Assert.True(retrieved);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Will_return_null_from_try_get_value_if_no_value_present()
        {
            var scenarioContext = CreateScenarioContext();

            ScenarioTestClass actual;
            var retrieved = scenarioContext.TryGetValue(out actual);

            Assert.False(retrieved);
            Assert.Null(actual);
        }

        [Fact]
        public void Can_retrieve_existing_value_with_try_get_value_and_generic_type()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(expected);

            IScenarioTestInterface actual;
            var retrieved = scenarioContext.TryGetValue(out actual);

            Assert.True(retrieved);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Can_retrieve_existing_value_with_try_get_value_and_string()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set(expected, "test");

            ScenarioTestClass actual;
            var retrieved = scenarioContext.TryGetValue("test", out actual);

            Assert.True(retrieved);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Will_not_retrieve_existing_value_with_different_key_for_try_get_value()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set(expected, "test");

            ScenarioTestClass actual;
            var retrieved = scenarioContext.TryGetValue("different", out actual);

            Assert.False(retrieved);
            Assert.Null(actual);
        }

        [Fact]
        public void Will_call_factory_method_when_retrieving_with_try_get_value()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set(() => expected);

            ScenarioTestClass actual;
            var retrieved = scenarioContext.TryGetValue(out actual);

            Assert.True(retrieved);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Will_call_factory_method_when_retrieving_with_try_get_value_and_generic_type()
        {
            var scenarioContext = CreateScenarioContext();
            var expected = new ScenarioTestClass();

            scenarioContext.Set<IScenarioTestInterface>(() => expected);

            IScenarioTestInterface actual;
            var retrieved = scenarioContext.TryGetValue(out actual);

            Assert.True(retrieved);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Will_handle_null_value_when_retrieving_with_try_get_value()
        {
            var scenarioContext = CreateScenarioContext();
            string expected = null;

            scenarioContext.Set(expected, "test");

            string actual;
            var retrieved = scenarioContext.TryGetValue("test", out actual);

            Assert.True(retrieved);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Can_get_and_set_a_null_value_with_an_object()
        {
            var scenarioContext = CreateScenarioContext();
            scenarioContext.Set<object>(null, "SomeKey");
            var result = scenarioContext.Get<object>("SomeKey");
            Assert.Null(result);
        }

        [Fact]
        public void Can_get_and_set_a_null_value_with_a_string()
        {
            var scenarioContext = CreateScenarioContext();
            scenarioContext.Set<string>(null, "SomeKey");
            var result = scenarioContext.Get<string>("SomeKey");
            Assert.Null(result);
        }

        private static ScenarioContext CreateScenarioContext()
        {
            return new ScenarioContext(new ObjectContainer(), new ScenarioInfo("Test", "Test Description", new string[] {}, null), new TestObjectResolver());
        }

        public class ScenarioTestClass : IScenarioTestInterface
        {
        }

        public interface IScenarioTestInterface
        {
        }
    }
}