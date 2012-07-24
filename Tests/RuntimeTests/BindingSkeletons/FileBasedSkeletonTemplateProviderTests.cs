using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.BindingSkeletons;
using Should;

namespace TechTalk.SpecFlow.RuntimeTests.BindingSkeletons
{
    [TestFixture]
    public class FileBasedSkeletonTemplateProviderTests
    {
        private class FileBasedSkeletonTemplateProviderStub : FileBasedSkeletonTemplateProvider
        {
            private readonly string templateFileContent;
            private readonly string missingTemplateResult;

            public FileBasedSkeletonTemplateProviderStub(string templateFileContent, string missingTemplateResult = null)
            {
                this.templateFileContent = templateFileContent;
                this.missingTemplateResult = missingTemplateResult;
            }

            protected override string GetTemplateFileContent()
            {
                return templateFileContent;
            }

            protected override string MissingTemplate(string key)
            {
                return missingTemplateResult;
            }
        }

        [Test]
        public void Should_parse_step_definition_class_template()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>CSharp/StepDefinitionClass
mytemplate
>>>other");

            var result = sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp);
            result.ShouldEqual("mytemplate" + Environment.NewLine);
        }

        [Test]
        public void Should_parse_step_definition_regex_template()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>CSharp/StepDefinitionRegex
mytemplate
>>>other");

            var result = sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, true);
            result.ShouldEqual("mytemplate" + Environment.NewLine);
        }

        [Test]
        public void Should_parse_step_definition_template()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>CSharp/StepDefinition
mytemplate
>>>other");

            var result = sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, false);
            result.ShouldEqual("mytemplate" + Environment.NewLine);
        }

        [Test]
        public void Should_handle_missing_step_definition_class_template()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>other", "missing");

            var result = sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp);
            result.ShouldEqual("missing");
        }

        [Test]
        public void Should_handle_missing_step_definition_regex_template()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>other", "missing");

            var result = sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, true);
            result.ShouldEqual("missing");
        }

        [Test]
        public void Should_handle_missing_step_definition_template()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>other", "missing");

            var result = sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, false);
            result.ShouldEqual("missing");
        }

        [Test]
        public void Should_handle_invalid_template_file_missing_title()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>
foo");

            Should.Assertions.Assert.Throws<SpecFlowException>(
                () => sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp));
        }

        [Test]
        public void Should_handle_invalid_template_file_duplicate_title()
        {
            var sut = new FileBasedSkeletonTemplateProviderStub(@">>>title1
foo
>>>title1
bar");

            Should.Assertions.Assert.Throws<SpecFlowException>(
                () => sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp));
        }

    }
}
