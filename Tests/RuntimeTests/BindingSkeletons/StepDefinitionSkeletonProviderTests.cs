using System;
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using Should;

namespace TechTalk.SpecFlow.RuntimeTests.BindingSkeletons
{
    [TestFixture]
    public class StepDefinitionSkeletonProviderTests
    {
        private Mock<ISkeletonTemplateProvider> templateProviderMock;

        [SetUp]
        public void Setup()
        {
            templateProviderMock = new Mock<ISkeletonTemplateProvider>();
            templateProviderMock.Setup(tp => tp.GetStepDefinitionClassTemplate(It.IsAny<ProgrammingLanguage>()))
                .Returns("{namespace}/{className}/{bindings}");
            templateProviderMock.Setup(tp => tp.GetStepDefinitionTemplate(It.IsAny<ProgrammingLanguage>(), true))
                .Returns("{attribute}/{regex}/{methodName}/{parameters}");
            templateProviderMock.Setup(tp => tp.GetStepDefinitionTemplate(It.IsAny<ProgrammingLanguage>(), false))
                .Returns("{attribute}/{methodName}/{parameters}");
        }

        private static StepInstance CreateSimpleWhen()
        {
            return new StepInstance(StepDefinitionType.When, StepDefinitionKeyword.When, "When ", "I do something", null, null, new StepContext("MyFeature", "MyScenario", null, new CultureInfo("en-US")));
        }

        private StepInstance CreateWhenWithExtraArgs()
        {
            var result = CreateSimpleWhen();
            result.TableArgument = new Table("foo", "bar");
            result.TableArgument.AddRow("v1", "v2");
            result.MultilineTextArgument = "multi-line-text";
            return result;
        }

        private class StepDefinitionSkeletonProviderStepDefinitionSkeletonStub : StepDefinitionSkeletonProvider
        {
            private readonly string stepDefinitionSkeleton;

            public StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(ISkeletonTemplateProvider templateProvider, string stepDefinitionSkeleton) : base(templateProvider)
            {
                this.stepDefinitionSkeleton = stepDefinitionSkeleton;
            }

            public override string GetStepDefinitionSkeleton(ProgrammingLanguage language, StepInstance stepInstance, StepDefinitionSkeletonStyle style)
            {
                return stepDefinitionSkeleton;
            }
        }

        [Test]
        public void Should_GetBindingClassSkeleton_generate_empty_step_definition_class()
        {
            var sut = new StepDefinitionSkeletonProvider(templateProviderMock.Object);

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new StepInstance[0], "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual("MyName.Space/MyClass/");
        }

        [Test]
        public void Should_GetBindingClassSkeleton_generate_single_step_definition_class()
        {
            var sut = new StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(templateProviderMock.Object, "step-definition-skeleton");

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new[] { CreateSimpleWhen() }, "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual(@"MyName.Space/MyClass/        step-definition-skeleton");
        }

        [Test]
        public void Should_GetBindingClassSkeleton_indent_step_definition_class()
        {
            var sut = new StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(templateProviderMock.Object, @"step-definition-skeleton
    inner-line
");

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new[] { CreateSimpleWhen() }, "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual(@"MyName.Space/MyClass/        step-definition-skeleton
            inner-line
");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_simple_regex_method()
        {
            var sut = new StepDefinitionSkeletonProvider(templateProviderMock.Object);

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen(), StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual("When/I do something/WhenIDoSomething/");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_regex_method_with_extra_args_csharp()
        {
            var sut = new StepDefinitionSkeletonProvider(templateProviderMock.Object);

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateWhenWithExtraArgs(), StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual("When/I do something/WhenIDoSomething/string multilineText, Table table");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_regex_method_with_extra_args_fsharp()
        {
            var sut = new StepDefinitionSkeletonProvider(templateProviderMock.Object);

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.FSharp, CreateWhenWithExtraArgs(), StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual("When/I do something/WhenIDoSomething/multilineText : string, table : Table");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_regex_method_with_extra_args_vb()
        {
            var sut = new StepDefinitionSkeletonProvider(templateProviderMock.Object);

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.VB, CreateWhenWithExtraArgs(), StepDefinitionSkeletonStyle.RegularExpressions);

            result.ShouldEqual("When/I do something/WhenIDoSomething/ByVal multilineText As String, ByVal table As Table");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_simple_underscored_method_name_method()
        {
            var sut = new StepDefinitionSkeletonProvider(templateProviderMock.Object);

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen(), StepDefinitionSkeletonStyle.MethodNameUnderscores);

            result.ShouldEqual("When/When_I_Do_Something/");
        }
    }
}
