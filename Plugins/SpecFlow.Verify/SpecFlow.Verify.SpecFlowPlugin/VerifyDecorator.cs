using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace SpecFlow.Verify.SpecFlowPlugin;

public class VerifyDecorator : ITestClassDecorator
{
    private readonly CodeDomHelper _codeDomHelper;

    public VerifyDecorator(CodeDomHelper codeDomHelper)
    {
        _codeDomHelper = codeDomHelper;
    }

    public int Priority { get; } = 0;

    public bool CanDecorateFrom(TestClassGenerationContext generationContext) => true;

    public void DecorateFrom(TestClassGenerationContext generationContext)
    {
        _codeDomHelper.AddAttribute(generationContext.TestClass, "UsesVerify");

    }
}
