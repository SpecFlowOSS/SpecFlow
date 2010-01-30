using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal interface IScenarioBuilder : IStepProcessor
    {
        Scenario GetResult();
    }
}