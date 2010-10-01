using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal interface IScenarioBuilder : IStepProcessor
    {
        string Title { get; }
        FilePosition Position { get; }
        Scenario GetResult();
    }
}