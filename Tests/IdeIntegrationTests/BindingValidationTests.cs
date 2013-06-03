namespace IdeIntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    
    using Moq;
    using NUnit.Framework;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Bindings;
    using TechTalk.SpecFlow.Bindings.Discovery;
    using TechTalk.SpecFlow.Bindings.Reflection;
    using TechTalk.SpecFlow.IdeIntegration.Bindings;
    using TechTalk.SpecFlow.IdeIntegration.Tracing;
    using TechTalk.SpecFlow.RuntimeTests;

    /// <summary>
    /// Tests binding validity in IDE integration
    /// </summary>
    [TestFixture]
    public class BindingValidationTests
    {
        /// <summary>
        /// The binding source processor that is currently being tested
        /// </summary>
        private BindingSourceProcessor BindingSourceProcessorUnderTest;

        /// <summary>
        /// A Binding attribute for the binding type
        /// </summary>
        private BindingSourceAttribute bindingSourceAttribute;

        [SetUp]
        public void Setup()
        {
            var ideTracerStub = new Mock<IIdeTracer>();
            this.BindingSourceProcessorUnderTest = new IdeBindingSourceProcessor(ideTracerStub.Object);
            this.bindingSourceAttribute = new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(BindingAttribute)) };
        }

        [Test]
        public void Class_should_be_valid()
        {
            var stubType = new BindingSourceType
                {
                    IsClass = true,
                    Attributes = new [] { this.bindingSourceAttribute }
                };

            var result = this.BindingSourceProcessorUnderTest.ProcessType(stubType);
            Assert.IsTrue(result, "Class should be a valid type");
        }

        [Test]
        public void Non_class_should__not_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsClass = false,
                Attributes = new[] { this.bindingSourceAttribute, }
            };

            var result = this.BindingSourceProcessorUnderTest.ProcessType(stubType);
            Assert.IsFalse(result, "Non-class should be not a valid type");
        }

        [Test]
        public void Non_generic_type_Definition_should_not_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsClass = true,
                IsGenericTypeDefinition = false,
                Attributes = new[] { this.bindingSourceAttribute, }
            };

            var result = this.BindingSourceProcessorUnderTest.ProcessType(stubType);
            Assert.IsTrue(result, "Non generic type definition should be a valid type");
        }

        [Test]
        public void Generic_type_Definition_should_not_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsClass = true,
                IsGenericTypeDefinition = true,
                Attributes = new[] { bindingSourceAttribute, }
            };

            var result = BindingSourceProcessorUnderTest.ProcessType(stubType);
            Assert.IsFalse(result, "Generic type definition should not be a valid type");
        }

        [Test]
        public void Non_static_method_in_non_abstract_class_should_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsGenericTypeDefinition = false,
                IsClass = true,
                Attributes = new[] { this.bindingSourceAttribute, }
            };

            var stubMethod = new BindingSourceMethod
                {
                    IsPublic = true,
                    IsStatic = false,
                    BindingMethod = new BindingMethod(new BindingType("Test", "Test"), "TestMethod", Enumerable.Empty<IBindingParameter>(), new BindingType("Test", "Test")),
                    Attributes = new[]
                        {
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(MethodBinding)) },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(StepDefinitionAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {} },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(ScopeAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {}},
                        }
                };

            var bspStub = new BindingSourceProcessorStub();

            // Process type must be called first to ensure the binding source processer is in the correct state
            bspStub.ProcessType(stubType);
            bspStub.ProcessMethod(stubMethod);

            Assert.True(bspStub.StepDefinitionBindings.Count > 0, "A non static method in a non-abstract class should be valid");
        }

        [Test]
        public void Non_static_method_in_abstract_class_should_not_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsGenericTypeDefinition = false,
                IsClass = true,
                IsAbstract = true,
                Attributes = new[] { bindingSourceAttribute, }
            };

            var stubMethod = new BindingSourceMethod
            {
                IsPublic = true,
                IsStatic = false,
                BindingMethod = new BindingMethod(new BindingType("Test", "Test"), "TestMethod", Enumerable.Empty<IBindingParameter>(), new BindingType("Test", "Test")),
                Attributes = new[]
                        {
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(MethodBinding)) },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(StepDefinitionAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {} },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(ScopeAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {}},
                        }
            };

            var bspStub = new BindingSourceProcessorStub();

            // Process type must be called first to ensure the binding source processer is in the correct state
            bspStub.ProcessType(stubType);
            bspStub.ProcessMethod(stubMethod);

            Assert.AreEqual(0, bspStub.StepDefinitionBindings.Count, "A non static method in an abstract class should not be valid");
        }

        [Test]
        public void Static_method_in_abstract_class_should_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsGenericTypeDefinition = false,
                IsClass = true,
                IsAbstract = true,
                Attributes = new[] { bindingSourceAttribute, }
            };

            var stubMethod = new BindingSourceMethod
            {
                IsPublic = true,
                IsStatic = true,
                BindingMethod = new BindingMethod(new BindingType("Test", "Test"), "TestMethod", Enumerable.Empty<IBindingParameter>(), new BindingType("Test", "Test")),
                Attributes = new[]
                        {
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(MethodBinding)) },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(StepDefinitionAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {} },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(ScopeAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {}},
                        }
            };

            var bspStub = new BindingSourceProcessorStub();

            // Process type must be called first to ensure the binding source processer is in the correct state
            bspStub.ProcessType(stubType);
            bspStub.ProcessMethod(stubMethod);

            Assert.True(bspStub.StepDefinitionBindings.Count > 0, "A static method in an abstract class should be valid");
        }

        [Test]
        public void Static_method_with_non_scenario_specific_hook_not_be_valid()
        {
                        var stubType = new BindingSourceType
            {
                IsGenericTypeDefinition = false,
                IsClass = true,
                IsAbstract = false,
                Attributes = new[] { bindingSourceAttribute, }
            };

            var stubMethod = new BindingSourceMethod
            {
                IsPublic = true,
                IsStatic = true,
                BindingMethod = new BindingMethod(new BindingType("Test", "Test"), "TestMethod", Enumerable.Empty<IBindingParameter>(), new BindingType("Test", "Test")),
                Attributes = new[]
                        {
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(BeforeTestRunAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {} },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(MethodBinding)) },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(StepDefinitionAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {} },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(ScopeAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] {}},
                        }
            };

            var bspStub = new BindingSourceProcessorStub();

            // Process type must be called first to ensure the binding source processer is in the correct state
            bspStub.ProcessType(stubType);
            bspStub.ProcessMethod(stubMethod);

            Assert.True(bspStub.HookBindings.Count > 0, "A static method with a non scenario specific hook should be valid");
        }

        [Test]
        public void Non_static_method_with_non_scenario_specific_hook_should_not_be_valid()
        {
            var stubType = new BindingSourceType
            {
                IsGenericTypeDefinition = false,
                IsClass = true,
                IsAbstract = false,
                Attributes = new[] { bindingSourceAttribute, }
            };

            var stubMethod = new BindingSourceMethod
            {
                IsPublic = true,
                IsStatic = false,
                BindingMethod = new BindingMethod(new BindingType("Test", "Test"), "TestMethod", Enumerable.Empty<IBindingParameter>(), new BindingType("Test", "Test")),
                Attributes = new[]
                        {
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(BeforeTestRunAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] { } },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(MethodBinding)) },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(StepDefinitionAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] { } },
                            new BindingSourceAttribute { AttributeType = new RuntimeBindingType(typeof(ScopeAttribute)), NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>(), AttributeValues = new IBindingSourceAttributeValueProvider[] { } },
                        }
            };

            var bspStub = new BindingSourceProcessorStub();

            // Process type must be called first to ensure the binding source processer is in the correct state
            bspStub.ProcessType(stubType);
            bspStub.ProcessMethod(stubMethod);

            Assert.AreEqual(0, bspStub.HookBindings.Count, "A non static method with a non scenario specific hook should not be valid");
        }
    }
}
