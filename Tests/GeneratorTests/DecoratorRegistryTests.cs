using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using BoDi;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using FluentAssertions;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class DecoratorRegistryTests
    {
        private IObjectContainer container;
        
        [SetUp]
        public void Setup()
        {
            container = new ObjectContainer();
        }

        internal static Mock<ITestClassTagDecorator> CreateTestClassTagDecoratorMock(string expectedTag = null)
        {
            var testClassDecoratorMock = new Mock<ITestClassTagDecorator>();
            testClassDecoratorMock.Setup(d => d.ApplyOtherDecoratorsForProcessedTags).Returns(false);
            testClassDecoratorMock.Setup(d => d.RemoveProcessedTags).Returns(true);
            testClassDecoratorMock.Setup(d => d.Priority).Returns(PriorityValues.Normal);
            if (expectedTag == null)
                testClassDecoratorMock.Setup(d => d.CanDecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>())).Returns(true);
            else
                testClassDecoratorMock.Setup(d => d.CanDecorateFrom(expectedTag, It.IsAny<TestClassGenerationContext>())).Returns(true);
            return testClassDecoratorMock;
        }

        internal static Mock<ITestClassDecorator> CreateTestClassDecoratorMock()
        {
            var testClassDecoratorMock = new Mock<ITestClassDecorator>();
            testClassDecoratorMock.Setup(d => d.Priority).Returns(PriorityValues.Normal);
            testClassDecoratorMock.Setup(d => d.CanDecorateFrom(It.IsAny<TestClassGenerationContext>())).Returns(true);
            return testClassDecoratorMock;
        }

        internal static Mock<ITestMethodDecorator> CreateTestMethodDecoratorMock()
        {
            var testClassDecoratorMock = new Mock<ITestMethodDecorator>();
            testClassDecoratorMock.Setup(d => d.Priority).Returns(PriorityValues.Normal);
            testClassDecoratorMock.Setup(d => d.CanDecorateFrom(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>())).Returns(true);
            return testClassDecoratorMock;
        }

        internal static Mock<ITestMethodTagDecorator> CreateTestMethodTagDecoratorMock(string expectedTag = null)
        {
            var testClassDecoratorMock = new Mock<ITestMethodTagDecorator>();
            testClassDecoratorMock.Setup(d => d.ApplyOtherDecoratorsForProcessedTags).Returns(false);
            testClassDecoratorMock.Setup(d => d.RemoveProcessedTags).Returns(true);
            testClassDecoratorMock.Setup(d => d.Priority).Returns(PriorityValues.Normal);
            if (expectedTag == null)
                testClassDecoratorMock.Setup(d => d.CanDecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>())).Returns(true);
            else
                testClassDecoratorMock.Setup(d => d.CanDecorateFrom(expectedTag, It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>())).Returns(true);
            return testClassDecoratorMock;
        }

        private DecoratorRegistry CreateDecoratorRegistry()
        {
            return new DecoratorRegistry(container);
        }

        private static TestClassGenerationContext CreateGenerationContext(string tag)
        {
            return new TestClassGenerationContext(null, new Feature { Tags = new Tags(new Tag(tag)) }, null, null, null, null, null, null, null, null, null, true, false);
        }

        [Test]
        public void Should_decorate_test_class()
        {
            var testClassDecoratorMock = CreateTestClassDecoratorMock();
            container.RegisterInstanceAs(testClassDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestClass(CreateGenerationContext("dummy"), out unprocessedTags);

            testClassDecoratorMock.Verify(d => d.DecorateFrom(It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_decorate_test_class_when_not_applicable()
        {
            var testClassDecoratorMock = CreateTestClassDecoratorMock();
            testClassDecoratorMock.Setup(d => d.CanDecorateFrom(It.IsAny<TestClassGenerationContext>())).Returns(false);
            container.RegisterInstanceAs(testClassDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestClass(CreateGenerationContext("dummy"), out unprocessedTags);

            testClassDecoratorMock.Verify(d => d.DecorateFrom(It.IsAny<TestClassGenerationContext>()), Times.Never());
        }

        [Test]
        public void Should_decorate_test_class_based_on_tag()
        {
            var testClassDecoratorMock = CreateTestClassTagDecoratorMock();
            container.RegisterInstanceAs(testClassDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestClass(CreateGenerationContext("foo"), out unprocessedTags);

            testClassDecoratorMock.Verify(d => d.DecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_remove_processed_tag_from_test_class_category_list()
        {
            var testClassDecoratorMock = CreateTestClassTagDecoratorMock();
            testClassDecoratorMock.Setup(d => d.RemoveProcessedTags).Returns(true);
            container.RegisterInstanceAs(testClassDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> classCats = null;
            registry.DecorateTestClass(CreateGenerationContext("foo"), out classCats);

            classCats.Should().NotBeNull();
            classCats.Should().NotContain("foo");
        }

        [Test]
        public void Should_keep_processed_tag_from_test_class_category_list()
        {
            var testClassDecoratorMock = CreateTestClassTagDecoratorMock();
            testClassDecoratorMock.Setup(d => d.RemoveProcessedTags).Returns(false);
            container.RegisterInstanceAs(testClassDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> classCats = null;
            registry.DecorateTestClass(CreateGenerationContext("foo"), out classCats);

            classCats.Should().NotBeNull();
            classCats.Should().Contain("foo");
        }

        [Test]
        public void Should_allow_multiple_decorators()
        {
            var testClassDecoratorMock1 = CreateTestClassTagDecoratorMock();
            testClassDecoratorMock1.Setup(d => d.ApplyOtherDecoratorsForProcessedTags).Returns(true);
            container.RegisterInstanceAs(testClassDecoratorMock1.Object, "foo1");

            var testClassDecoratorMock2 = CreateTestClassTagDecoratorMock();
            testClassDecoratorMock2.Setup(d => d.ApplyOtherDecoratorsForProcessedTags).Returns(true);
            container.RegisterInstanceAs(testClassDecoratorMock2.Object, "foo2");

            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestClass(CreateGenerationContext("foo"), out unprocessedTags);

            testClassDecoratorMock1.Verify(d => d.DecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>()));
            testClassDecoratorMock2.Verify(d => d.DecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_higher_priority_decorator_applied_first()
        {
            List<string> executionOrder = new List<string>();

            var testClassDecoratorMock1 = CreateTestClassTagDecoratorMock();
            testClassDecoratorMock1.Setup(d => d.ApplyOtherDecoratorsForProcessedTags).Returns(true);
            testClassDecoratorMock1.Setup(d => d.DecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>()))
                .Callback((string t, TestClassGenerationContext c) => executionOrder.Add("foo1"));

            container.RegisterInstanceAs(testClassDecoratorMock1.Object, "foo1");

            var testClassDecoratorMock2 = CreateTestClassTagDecoratorMock();
            testClassDecoratorMock2.Setup(d => d.ApplyOtherDecoratorsForProcessedTags).Returns(true);
            testClassDecoratorMock2.Setup(d => d.Priority).Returns(PriorityValues.High);
            testClassDecoratorMock2.Setup(d => d.DecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>()))
                .Callback((string t, TestClassGenerationContext c) => executionOrder.Add("foo2"));
            
            container.RegisterInstanceAs(testClassDecoratorMock2.Object, "foo2");

            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestClass(CreateGenerationContext("foo"), out unprocessedTags);

            executionOrder.Should().Equal(new object[] { "foo2", "foo1" });
        }

        [Test]
        public void Should_not_decorate_test_method_for_feature_tag()
        {
            var testMethodDecoratorMock = CreateTestMethodTagDecoratorMock();
            container.RegisterInstanceAs(testMethodDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestMethod(CreateGenerationContext("foo"), null, new Tag[] { }, out unprocessedTags);

            testMethodDecoratorMock.Verify(d => d.DecorateFrom("foo", It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()), Times.Never());
        }

        [Test]
        public void Should_decorate_test_method_for_scenario_tag()
        {
            var testMethodDecoratorMock = CreateTestMethodTagDecoratorMock();
            container.RegisterInstanceAs(testMethodDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestMethod(CreateGenerationContext("dummy"), null, new Tag[] { new Tag("foo") }, out unprocessedTags);

            testMethodDecoratorMock.Verify(d => d.DecorateFrom(It.IsAny<string>(), It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }

        [Test]
        public void Should_decorate_test_method()
        {
            var testMethodDecoratorMock = CreateTestMethodDecoratorMock();
            container.RegisterInstanceAs(testMethodDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestMethod(CreateGenerationContext("dummy"), null, new Tag[] { new Tag("dummy") }, out unprocessedTags);

            testMethodDecoratorMock.Verify(d => d.DecorateFrom(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }

        [Test]
        public void Should_not_decorate_test_method_when_not_applicable()
        {
            var testMethodDecoratorMock = CreateTestMethodDecoratorMock();
            testMethodDecoratorMock.Setup(d => d.CanDecorateFrom(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()))
                .Returns(false);
            container.RegisterInstanceAs(testMethodDecoratorMock.Object, "foo");
            var registry = CreateDecoratorRegistry();

            List<string> unprocessedTags;
            registry.DecorateTestMethod(CreateGenerationContext("dummy"), null, new Tag[] { new Tag("dummy") }, out unprocessedTags);

            testMethodDecoratorMock.Verify(d => d.DecorateFrom(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()), Times.Never());
        }
    }

    internal class DecoratorRegistryStub : IDecoratorRegistry
    {
        public void DecorateTestClass(TestClassGenerationContext generationContext, out List<string> unprocessedTags)
        {
            unprocessedTags = new List<string>();
        }

        public void DecorateTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            unprocessedTags = new List<string>();
        }
    }
}
