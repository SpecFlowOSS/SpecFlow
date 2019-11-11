namespace TechTalk.SpecFlow.Specs.Support
{
    [Binding]
    public class StepArgumentTransformations
    {

        [StepArgumentTransformation(@"enabled")]
        public bool ConvertEnabled() { return true; }

        [StepArgumentTransformation(@"disabled")]
        public bool ConvertDisabled() { return false; }
    }
}
