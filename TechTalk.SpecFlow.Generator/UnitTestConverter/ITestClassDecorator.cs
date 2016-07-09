using System;
using System.CodeDom;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface ITestClassTagDecorator
    {
        int Priority { get; }
        bool RemoveProcessedTags { get; }
        bool ApplyOtherDecoratorsForProcessedTags { get; }

        bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext);
        void DecorateFrom(string tagName, TestClassGenerationContext generationContext);
    }

    public interface ITestMethodTagDecorator
    {
        int Priority { get; }
        bool RemoveProcessedTags { get; }
        bool ApplyOtherDecoratorsForProcessedTags { get; }

        bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod);
        void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod);
    }

    public interface ITestClassDecorator
    {
        int Priority { get; }

        bool CanDecorateFrom(TestClassGenerationContext generationContext);
        void DecorateFrom(TestClassGenerationContext generationContext);
    }

    public interface ITestMethodDecorator
    {
        int Priority { get; }

        bool CanDecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod);
        void DecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod);
    }
}
