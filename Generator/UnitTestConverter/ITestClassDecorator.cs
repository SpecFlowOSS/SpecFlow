using System;
using System.CodeDom;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface ITestClassDecorator
    {
        int Priority { get; }
        bool RemoveProcessedTags { get; }
        bool ApplyOtherDecoratorsForProcessedTags { get; }

        bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, string registeredName);
        void DecorateFrom(string tagName, TestClassGenerationContext generationContext, string registeredName);
    }

    public interface ITestMethodDecorator
    {
        int Priority { get; }
        bool RemoveProcessedTags { get; }
        bool ApplyOtherDecoratorsForProcessedTags { get; }

        bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string registeredName);
        void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string registeredName);
    }
}
