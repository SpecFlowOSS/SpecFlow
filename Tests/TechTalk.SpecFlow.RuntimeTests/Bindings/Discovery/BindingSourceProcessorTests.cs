using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings.Discovery
{
    public class BindingSourceProcessorTests
    {
        [Fact]
        public void ProcessTypeAndMethod_InVisualStudioExtension_ShouldFindBinding()
        {
            // The Visual Studio Extension uses a source code based binding reflection that
            //   * cannot support IPolymorphicBindingType
            //   * resolves the BindingSourceAttribute from the step definition source code without the Types property

            //ARRANGE
            var sut = CreateBindingSourceProcessor();

            var bindingSourceType = new BindingSourceType
            {
                Attributes = new[]
                {
                    CreateBindingSourceAttribute("BindingAttribute", "TechTalk.SpecFlow.BindingAttribute")
                },
            };

            var bindingSourceMethod = new BindingSourceMethod
            {
                BindingMethod = CreateBindingMethodStub(),
                Attributes = new[]
                {
                    CreateBindingSourceAttribute("GivenAttribute", "TechTalk.SpecFlow.GivenAttribute")
                        .WithValue("an authenticated user"),
                }
            };

            //ACT
            sut.ProcessType(bindingSourceType);
            sut.ProcessMethod(bindingSourceMethod);
            sut.BuildingCompleted();

            //ASSERT
            var binding = sut.StepDefinitionBindings.Should().ContainSingle().Subject;
            binding.StepDefinitionType.Should().Be(StepDefinitionType.Given);
            binding.Regex.Should().NotBeNull();
            binding.Regex.IsMatch("an authenticated user").Should().BeTrue();
        }

        private static BindingSourceAttribute CreateBindingSourceAttribute(string name, string fullName) => new()
        {
            AttributeType = new BindingType(name, fullName, "default"),
            AttributeValues = Array.Empty<IBindingSourceAttributeValueProvider>(),
            NamedAttributeValues = new Dictionary<string, IBindingSourceAttributeValueProvider>()
        };

        private BindingSourceProcessorStub CreateBindingSourceProcessor()
        {
            //NOTE: BindingSourceProcessor is abstract, to test its base functionality we need to instantiate a subclass
            return new BindingSourceProcessorStub();
        }

        private BindingMethod CreateBindingMethodStub()
        {
            var bindingType = new BindingType("MyType", "MyProject.MyType", "MyProject");
            return new BindingMethod(bindingType, "MyMethod", Array.Empty<IBindingParameter>(), RuntimeBindingType.Void);
        }
    }

    public static class BindingSourceTestExtensions
    {
        public static BindingSourceAttribute WithValue(this BindingSourceAttribute a, string val)
        {
            var values = a.AttributeValues == null ? new List<IBindingSourceAttributeValueProvider>() : a.AttributeValues.ToList();

            values.Add(new BindingSourceAttributeValueProvider(val));

            a.AttributeValues = values.ToArray();
            return a;
        }
    }
}
