namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal interface IStepProcessor
    {
        void ProcessStep(StepBuilder step);
    }
}