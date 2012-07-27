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
        private Mock<IStepTextAnalyzer> stepTextAnalyzerMock;
        private AnalyzedStepText analizeResult;
        private readonly CultureInfo bindingCulture = new CultureInfo("en-US");

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

            analizeResult = new AnalyzedStepText();
            stepTextAnalyzerMock = new Mock<IStepTextAnalyzer>();
            stepTextAnalyzerMock.Setup(a => a.Analyze(It.IsAny<string>(), It.IsAny<CultureInfo>()))
                .Returns(analizeResult);
        }

        private StepInstance CreateSimpleWhen(string text = "I do something")
        {
            var result = new StepInstance(StepDefinitionType.When, StepDefinitionKeyword.When, "When ", text, null, null, new StepContext("MyFeature", "MyScenario", null, new CultureInfo("en-US")));
            analizeResult.TextParts.Add(text);
            return result;
        }

        private StepInstance CreateSimpleGiven(string text = "I had something")
        {
            var result = new StepInstance(StepDefinitionType.Given, StepDefinitionKeyword.Given, "Given ", text, null, null, new StepContext("MyFeature", "MyScenario", null, new CultureInfo("en-US")));
            analizeResult.TextParts.Add(text);
            return result;
        }

        private StepInstance CreateSimpleThen(string text = "I have something")
        {
            var result = new StepInstance(StepDefinitionType.Then, StepDefinitionKeyword.Then, "Then ", text, null, null, new StepContext("MyFeature", "MyScenario", null, new CultureInfo("en-US")));
            analizeResult.TextParts.Add(text);
            return result;
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
            private int usageIndex = 0;
            private readonly string[] stepDefinitionSkeletons;
            private Func<StepInstance, string> getSkeleton;

            public StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(ISkeletonTemplateProvider templateProvider, IStepTextAnalyzer stepTextAnalyzer, params string[] stepDefinitionSkeletons) : base(templateProvider, stepTextAnalyzer)
            {
                this.stepDefinitionSkeletons = stepDefinitionSkeletons;
            }

            public StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(ISkeletonTemplateProvider templateProvider, IStepTextAnalyzer stepTextAnalyzer, Func<StepInstance, string> getSkeleton) : base(templateProvider, stepTextAnalyzer)
            {
                this.getSkeleton = getSkeleton;
            }

            public override string GetStepDefinitionSkeleton(ProgrammingLanguage language, StepInstance stepInstance, StepDefinitionSkeletonStyle style, CultureInfo bindingCulture)
            {
                if (getSkeleton != null)
                    return getSkeleton(stepInstance);

                if (usageIndex < stepDefinitionSkeletons.Length)
                {
                    return stepDefinitionSkeletons[usageIndex++];
                }

                return stepDefinitionSkeletons[stepDefinitionSkeletons.Length - 1];
            }
        }

        private StepDefinitionSkeletonProvider CreateSut()
        {
            return new StepDefinitionSkeletonProvider(templateProviderMock.Object, stepTextAnalyzerMock.Object);
        }

        [Test]
        public void Should_GetBindingClassSkeleton_generate_empty_step_definition_class()
        {
            var sut = CreateSut();

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new StepInstance[0], "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual("MyName.Space/MyClass/");
        }

        [Test]
        public void Should_GetBindingClassSkeleton_generate_single_step_definition_class()
        {
            var sut = new StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(templateProviderMock.Object, stepTextAnalyzerMock.Object, "step-definition-skeleton");

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new[] { CreateSimpleWhen() }, "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual(@"MyName.Space/MyClass/        step-definition-skeleton");
        }

        [Test]
        public void Should_GetBindingClassSkeleton_merges_same_step_definition_methods()
        {
            var sut = new StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(templateProviderMock.Object, stepTextAnalyzerMock.Object, "step-definition-skeleton", "other-step-definition-skeleton", "step-definition-skeleton");

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new[] { CreateSimpleWhen(), CreateSimpleWhen(), CreateSimpleWhen() }, "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual(@"MyName.Space/MyClass/        step-definition-skeleton
        other-step-definition-skeleton");
        }

        [Test]
        public void Should_GetBindingClassSkeleton_orders_step_definition_methods_by_type()
        {
            var sut = new StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(templateProviderMock.Object, stepTextAnalyzerMock.Object, si => si.StepDefinitionType.ToString() + "-skeleton");

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new[] { CreateSimpleWhen(), CreateSimpleThen(), CreateSimpleGiven() }, "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual(@"MyName.Space/MyClass/        Given-skeleton
        When-skeleton
        Then-skeleton");
        }

        [Test]
        public void Should_GetBindingClassSkeleton_indent_step_definition_class()
        {
            var sut = new StepDefinitionSkeletonProviderStepDefinitionSkeletonStub(templateProviderMock.Object, stepTextAnalyzerMock.Object, @"step-definition-skeleton
    inner-line
");

            var result = sut.GetBindingClassSkeleton(ProgrammingLanguage.CSharp, new[] { CreateSimpleWhen() }, "MyName.Space", "MyClass", StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual(@"MyName.Space/MyClass/        step-definition-skeleton
            inner-line");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_simple_regex_style()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen(), StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual("When/I do something/WhenIDoSomething/");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_regex_style_with_extra_args_csharp()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateWhenWithExtraArgs(), StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual("When/I do something/WhenIDoSomething/string multilineText, Table table");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_regex_style_with_extra_args_fsharp()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.FSharp, CreateWhenWithExtraArgs(), StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual("When/I do something/WhenIDoSomething/multilineText : string, table : Table");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_regex_style_with_extra_args_vb()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.VB, CreateWhenWithExtraArgs(), StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual("When/I do something/WhenIDoSomething/ByVal multilineText As String, ByVal table As Table");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_exclude_parameter_from_method_name_regex_style()
        {
            var sut = CreateSut();

            var stepInstance = CreateWhenWithSingleParam();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, stepInstance, StepDefinitionSkeletonStyle.RegexAttribute, bindingCulture);

            result.ShouldEqual("When/I '(.*)' something/WhenISomething/string p0");
        }

        private StepInstance CreateWhenWithSingleParam()
        {
            var stepInstance = CreateSimpleWhen("I 'do' something");
            analizeResult.TextParts.Clear();
            analizeResult.TextParts.Add("I '");
            analizeResult.TextParts.Add("' something");
            analizeResult.Parameters.Add(new AnalyzedStepParameter("String", "p0", ".*"));
            return stepInstance;
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_simple_underscored_method_name_style()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen(), StepDefinitionSkeletonStyle.MethodNameUnderscores, bindingCulture);

            result.ShouldEqual("When/When_I_do_something/");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_punctuated_underscored_method_name_style()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen("I, do-something."), StepDefinitionSkeletonStyle.MethodNameUnderscores, bindingCulture);

            result.ShouldEqual("When/When_I_do_something/");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_parametrized_underscored_method_name_style()
        {
            var sut = CreateSut();

            var stepInstance = CreateWhenWithSingleParam();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, stepInstance, StepDefinitionSkeletonStyle.MethodNameUnderscores, bindingCulture);

            result.ShouldEqual("When/When_I_P0_something/string p0");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_non_latin_underscored_method_name_style()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen("Árvíztűrő tükörfúrógép"), StepDefinitionSkeletonStyle.MethodNameUnderscores, bindingCulture);

            result.ShouldEqual("When/When_Árvíztűrő_tükörfúrógép/");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_simple_pascal_case_method_name_style()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, CreateSimpleWhen(), StepDefinitionSkeletonStyle.MethodNamePascalCase, bindingCulture);

            result.ShouldEqual("When/WhenIDoSomething/");
        }

        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_parametrized_pascal_case_method_name_style()
        {
            var sut = CreateSut();

            var stepInstance = CreateWhenWithSingleParam();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, stepInstance, StepDefinitionSkeletonStyle.MethodNamePascalCase, bindingCulture);

            result.ShouldEqual("When/WhenI_P0_Something/string p0");
        }


        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_parametrized_pascal_case_method_name_style_with_parameter_at_the_end()
        {
            var sut = CreateSut();

            var stepInstance = CreateSimpleWhen("I do 42");
            analizeResult.TextParts.Clear();
            analizeResult.TextParts.Add("I do ");
            analizeResult.TextParts.Add("");
            analizeResult.Parameters.Add(new AnalyzedStepParameter("Int32", "p0", @"\d+"));

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.CSharp, stepInstance, StepDefinitionSkeletonStyle.MethodNamePascalCase, bindingCulture);

            result.ShouldEqual("When/WhenIDo_P0/int p0");
        }


        [Test]
        public void Should_GetStepDefinitionSkeleton_generate_simple_regex_method_name_style()
        {
            var sut = CreateSut();

            var result = sut.GetStepDefinitionSkeleton(ProgrammingLanguage.FSharp, CreateSimpleWhen(), StepDefinitionSkeletonStyle.MethodNameRegex, bindingCulture);

            result.ShouldEqual("When/``I do something``/");
        }
    }
}
