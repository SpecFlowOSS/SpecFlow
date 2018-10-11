using System.Linq;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class ServiceComponentListTests
    {
        [Fact]
        public void Should_be_empty_when_created()
        {
            var sut = new ServiceComponentList<ITestComponent>();

            sut.Should().BeEmpty();
        }

        [Fact]
        public void Should_allow_the_addition_of_new_components()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var component = new TestComponentImpl();
            sut.Register(component);

            sut.Should().Equal(component);
        }

        [Fact]
        public void Should_allow_the_addition_of_new_components_via_generic_overload()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            sut.Register<TestComponentImpl>();

            sut.Should().NotBeEmpty();
            sut.Should().AllBeOfType<TestComponentImpl>();
        }

        [Fact]
        public void Should_reverse_registration_order_of_added_components()
        {
            var sut = new ServiceComponentList<ITestComponent>();

            var components = Enumerable.Range(0, 100).Select(_ => new TestComponentImpl()).ToList();
            components.ForEach(sut.Register);

            components.Reverse();
            sut.Should().Equal(components);
        }

        [Fact]
        public void Should_allow_the_addition_of_default_components_at_the_end_of_the_list()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            var @default = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault(@default);

            sut.Should().Equal(registeredLast, registeredFirst, @default);
        }

        [Fact]
        public void Should_allow_the_addition_of_default_components_at_the_end_of_the_list_via_generic_overload()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault<AnotherImpl>();

            sut.Last().Should().BeOfType<AnotherImpl>();
        }

        [Fact]
        public void Should_allow_the_replacement_of_default_components_at_the_end_of_the_list()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            var @default = new TestComponentImpl();
            var @default2 = new AnotherImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault(@default);
            sut.SetDefault(@default2);

            sut.Should().Equal(registeredLast, registeredFirst, @default2);
        }

        [Fact]
        public void Should_allow_the_replacement_of_default_components_at_the_end_of_the_list_via_generic_overload()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault<TestComponentImpl>();
            sut.SetDefault<AnotherImpl>();

            sut.Last().Should().BeOfType<AnotherImpl>();
        }

        [Fact]
        public void Should_allow_the_clearing_of_default_components()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            var @default = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault(@default);
            sut.ClearDefault();

            sut.Should().Equal(registeredLast, registeredFirst);
        }

        [Fact]
        public void Should_allow_the_removal_of_default_component_via_generic_unregistration()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.SetDefault<AnotherImpl>();
            sut.Unregister<ITestComponent>();

            sut.Should().BeEmpty();
        }

        [Fact]
        public void Should_allow_the_unregistration_of_default_components()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            var @default = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault(@default);
            sut.Unregister(@default);

            sut.Should().Equal(registeredLast, registeredFirst);
        }

        [Fact]
        public void Should_allow_unregistration_of_existing_component()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var component = new TestComponentImpl();
            sut.Register(component);
            sut.Unregister(component);

            sut.Should().BeEmpty();
        }

        [Fact]
        public void Should_allow_clearing_all_components()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var registeredFirst = new TestComponentImpl();
            var registeredLast = new TestComponentImpl();
            var @default = new TestComponentImpl();
            sut.Register(registeredFirst);
            sut.Register(registeredLast);
            sut.SetDefault(@default);
            sut.Clear();

            sut.Should().BeEmpty();
        }

        [Fact]
        public void Should_allow_unregistration_of_existing_component_via_generic_overload()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            sut.Register(new TestComponentImpl());
            sut.Unregister<TestComponentImpl>();

            sut.Should().BeEmpty();
        }

        [Fact]
        public void Should_ignore_unregistration_of_nonexisting_component()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            sut.Register(new TestComponentImpl());
            sut.Unregister(new TestComponentImpl());

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_ignore_unregistration_of_nonexisting_component_via_generic_overload()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            sut.Register(new TestComponentImpl());
            sut.Unregister<AnotherImpl>();

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_allow_to_replace_a_component_with_another_one()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            var oldComponent = new TestComponentImpl();
            var newComponent = new TestComponentImpl();
            sut.Register(oldComponent);
            sut.Replace(oldComponent, newComponent);

            sut.Should().Equal(newComponent);
        }

        [Fact]
        public void Should_allow_to_replace_a_component_with_another_one_via_generic_overload()
        {
            var sut = new ServiceComponentList<ITestComponent>();
            sut.Register(new TestComponentImpl());
            sut.Replace<TestComponentImpl, AnotherImpl>();

            sut.Should().NotBeEmpty();
            sut.Should().AllBeOfType<AnotherImpl>();
        }

        private interface ITestComponent
        {
        }

        private class TestComponentImpl : ITestComponent
        {
        }

        private class AnotherImpl : ITestComponent
        {
        }
    }
}