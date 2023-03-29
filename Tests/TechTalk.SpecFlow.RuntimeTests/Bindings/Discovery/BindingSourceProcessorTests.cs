using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var bindingSourceType = CreateSyntheticBindingSourceType();
            var bindingSourceMethod = CreateSyntheticStepDefBindingSourceMethod();

            //ACT
            sut.ProcessType(bindingSourceType).Should().BeTrue();
            sut.ProcessMethod(bindingSourceMethod);
            sut.BuildingCompleted();

            //ASSERT
            var binding = sut.StepDefinitionBindings.Should().ContainSingle().Subject;
            binding.StepDefinitionType.Should().Be(StepDefinitionType.Given);
            binding.Regex.Should().NotBeNull();
            binding.Regex.IsMatch("an authenticated user").Should().BeTrue();
        }

        private BindingSourceMethod CreateSyntheticStepDefBindingSourceMethod()
        {
            return new BindingSourceMethod
            {
                BindingMethod = CreateSyntheticBindingMethod(),
                Attributes = new[]
                {
                    CreateBindingSourceAttribute("GivenAttribute", "TechTalk.SpecFlow.GivenAttribute")
                        .WithValue("an authenticated user"),
                }
            };
        }

        private BindingSourceMethod CreateSyntheticHookBindingSourceMethod(HookType hookType)
        {
            return new BindingSourceMethod
            {
                BindingMethod = CreateSyntheticBindingMethod(),
                Attributes = new[]
                {
                    CreateBindingSourceAttribute($"{hookType}Attribute", $"TechTalk.SpecFlow.{hookType}Attribute")
                }
            };
        }

        private BindingSourceType CreateSyntheticBindingSourceType()
        {
            var bindingSourceType = new BindingSourceType
            {
                IsClass = true,
                Attributes = new[]
                {
                    CreateBindingSourceAttribute("BindingAttribute", "TechTalk.SpecFlow.BindingAttribute")
                },
            };
            return bindingSourceType;
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

        private BindingMethod CreateSyntheticBindingMethod()
        {
            var bindingType = new BindingType("MyType", "MyProject.MyType", "MyProject");
            return new BindingMethod(bindingType, "MyMethod", Array.Empty<IBindingParameter>(), RuntimeBindingType.Void);
        }

        [Binding]
        class StepDefClassWithAsyncVoid
        {
            [Given("an authenticated user")]
            public async void AsyncVoidStepDef()
            {
                await Task.Delay(50); // we need to wait a bit otherwise the assertion passes even if the method is called sync
            }
        }

        [Fact]
        public async Task Async_void_binding_methods_are_not_supported()
        {
            var sut = CreateBindingSourceProcessor();

            var bindingSourceMethod = CreateBindingSourceMethod(typeof(StepDefClassWithAsyncVoid), nameof(StepDefClassWithAsyncVoid.AsyncVoidStepDef),
                CreateBindingSourceAttribute("GivenAttribute", "TechTalk.SpecFlow.GivenAttribute").WithValue("an authenticated user"));

            sut.ProcessType(CreateSyntheticBindingSourceType()).Should().BeTrue();
            sut.ProcessMethod(bindingSourceMethod);
            sut.BuildingCompleted();

            sut.ValidationErrors.Should().Contain(m => m.Contains("async void"));
        }

        [Theory]
        [InlineData(false, false, "Binding types must be classes")]
        [InlineData(true, true, "Binding types cannot be generic")]
        public void Binding_type_errors_should_be_captured(bool isClass, bool isGenericTypeDefinition, string expectedError)
        {
            var sut = CreateBindingSourceProcessor();

            var bindingSourceType = CreateSyntheticBindingSourceType();
            // make it invalid
            bindingSourceType.IsClass = isClass;
            bindingSourceType.IsGenericTypeDefinition = isGenericTypeDefinition;

            sut.ProcessType(bindingSourceType).Should().BeFalse();
            sut.BuildingCompleted();

            sut.ValidationErrors.Should().Contain(m => m.Contains(expectedError));
        }

        [Theory]
        [InlineData(true, false, "Abstract binding types can only have static binding methods")]
        public void Binding_method_errors_should_be_captured(bool isClassAbstract, bool isMethodStatic, string expectedError)
        {
            var sut = CreateBindingSourceProcessor();

            var bindingSourceType = CreateSyntheticBindingSourceType();
            bindingSourceType.IsAbstract = isClassAbstract;
            var bindingSourceMethod = CreateSyntheticStepDefBindingSourceMethod();
            bindingSourceMethod.IsStatic = isMethodStatic;

            sut.ProcessType(bindingSourceType).Should().BeTrue();
            sut.ProcessMethod(bindingSourceMethod);
            sut.BuildingCompleted();

            sut.ValidationErrors.Should().Contain(m => m.Contains(expectedError));
        }

        [Theory]
        [InlineData(HookType.BeforeTestRun)]
        [InlineData(HookType.AfterTestRun)]
        [InlineData(HookType.BeforeFeature)]
        [InlineData(HookType.AfterFeature)]
        public void Non_static_feature_and_test_run_hook_errors_should_be_captured(HookType hookType)
        {
            var sut = CreateBindingSourceProcessor();

            var bindingSourceType = CreateSyntheticBindingSourceType();
            var bindingSourceMethod = CreateSyntheticHookBindingSourceMethod(hookType);
            bindingSourceMethod.IsStatic = false;

            sut.ProcessType(bindingSourceType).Should().BeTrue();
            sut.ProcessMethod(bindingSourceMethod);
            sut.BuildingCompleted();

            sut.ValidationErrors.Should().Contain(m => m.Contains("The binding methods for before/after feature and before/after test run events must be static"));
        }

        private static BindingSourceMethod CreateBindingSourceMethod(Type bindingType, string methodName, params BindingSourceAttribute[] attributes)
        {
            var methodInfo = bindingType.GetMethod(methodName);
            return new BindingSourceMethod
            {
                BindingMethod = new RuntimeBindingMethod(methodInfo),
                IsPublic = methodInfo!.IsPublic,
                IsStatic = methodInfo!.IsStatic,
                Attributes = attributes ?? Array.Empty<BindingSourceAttribute>()
            };
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
