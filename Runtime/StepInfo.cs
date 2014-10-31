namespace TechTalk.SpecFlow
{
    using TechTalk.SpecFlow.Bindings;

    public class StepInfo
    {
        public StepDefinitionType StepDefinitionType { get; private set; }

        public string Text { get; private set; }

        public StepInfo(StepDefinitionType stepDefinitionType, string text)
        {
            this.StepDefinitionType = stepDefinitionType;
            this.Text = text;
        }
    }
}