using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace SpecFlow.Verify.SpecFlowPlugin;

public class VerifyDecorator : ITestClassTagDecorator
{
    private readonly CodeDomHelper _codeDomHelper;

    public VerifyDecorator(CodeDomHelper codeDomHelper)
    {
        _codeDomHelper = codeDomHelper;
    }

    public int Priority { get; }

    public bool RemoveProcessedTags { get; }

    public bool ApplyOtherDecoratorsForProcessedTags { get; }

    public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext) => true;

    public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
    {
        _codeDomHelper.AddAttribute(generationContext.TestClass, "UsesVerify");
    }
}
