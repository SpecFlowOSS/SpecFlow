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
    public class ResourceSkeletonTemplateProviderTests
    {
        private void ShouldNotBeMissing(string template)
        {
            template.ShouldNotBeNull();
            template.ShouldNotEqual("");
            template.ShouldNotEqual("undefined template");
        }

        [Test]
        public void Should_provide_csharp_templates()
        {
            var sut = new ResourceSkeletonTemplateProvider();

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, false));
        }

        [Test]
        public void Should_provide_vb_templates()
        {
            var sut = new ResourceSkeletonTemplateProvider();

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.VB));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, false));
        }

        [Test]
        public void Should_provide_fsharp_templates()
        {
            var sut = new ResourceSkeletonTemplateProvider();

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.FSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, false));
        }
    }

    [TestFixture]
    public class DefaultSkeletonTemplateProviderTests
    {
        private void ShouldNotBeMissing(string template)
        {
            template.ShouldNotBeNull();
            template.ShouldNotEqual("");
            template.ShouldNotEqual("undefined template");
        }

        [Test]
        public void Should_provide_csharp_templates()
        {
            var sut = new DefaultSkeletonTemplateProvider(new ResourceSkeletonTemplateProvider());

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, false));
        }

        [Test]
        public void Should_provide_vb_templates()
        {
            var sut = new DefaultSkeletonTemplateProvider(new ResourceSkeletonTemplateProvider());

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.VB));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, false));
        }

        [Test]
        public void Should_provide_fsharp_templates()
        {
            var sut = new DefaultSkeletonTemplateProvider(new ResourceSkeletonTemplateProvider());

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.FSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, false));
        }
    }
}
