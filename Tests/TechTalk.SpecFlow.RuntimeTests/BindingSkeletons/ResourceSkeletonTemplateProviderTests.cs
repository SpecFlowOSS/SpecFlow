using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using TechTalk.SpecFlow.BindingSkeletons;
using FluentAssertions;

namespace TechTalk.SpecFlow.RuntimeTests.BindingSkeletons
{
    
    public class ResourceSkeletonTemplateProviderTests
    {
        private void ShouldNotBeMissing(string template)
        {
            template.Should().NotBeNull();
            template.Should().NotBeEmpty();
            template.Should().NotBe("undefined template");
        }

        [Fact]
        public void Should_provide_csharp_templates()
        {
            var sut = new ResourceSkeletonTemplateProvider();

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, false));
        }

        [Fact]
        public void Should_provide_vb_templates()
        {
            var sut = new ResourceSkeletonTemplateProvider();

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.VB));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, false));
        }

        [Fact]
        public void Should_provide_fsharp_templates()
        {
            var sut = new ResourceSkeletonTemplateProvider();

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.FSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, false));
        }
    }

    
    public class DefaultSkeletonTemplateProviderTests
    {
        private void ShouldNotBeMissing(string template)
        {
            template.Should().NotBeNull();
            template.Should().NotBeEmpty();
            template.Should().NotBe("undefined template");
        }

        [Fact]
        public void Should_provide_csharp_templates()
        {
            var sut = new DefaultSkeletonTemplateProvider(new ResourceSkeletonTemplateProvider());

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.CSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.CSharp, false));
        }

        [Fact]
        public void Should_provide_vb_templates()
        {
            var sut = new DefaultSkeletonTemplateProvider(new ResourceSkeletonTemplateProvider());

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.VB));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.VB, false));
        }

        [Fact]
        public void Should_provide_fsharp_templates()
        {
            var sut = new DefaultSkeletonTemplateProvider(new ResourceSkeletonTemplateProvider());

            ShouldNotBeMissing(sut.GetStepDefinitionClassTemplate(ProgrammingLanguage.FSharp));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, true));
            ShouldNotBeMissing(sut.GetStepDefinitionTemplate(ProgrammingLanguage.FSharp, false));
        }
    }
}
