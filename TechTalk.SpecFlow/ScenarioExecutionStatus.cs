namespace TechTalk.SpecFlow
{
    public enum ScenarioExecutionStatus
    {
        OK,
        StepDefinitionPending,
        UndefinedStep,
        BindingError,
        TestError
    }
}