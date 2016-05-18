namespace TechTalk.SpecFlow.Infrastructure
{
    public enum TestStatus
    {
        OK,
        StepDefinitionPending,
        MissingStepDefinition,
        BindingError,
        TestError
    }
}