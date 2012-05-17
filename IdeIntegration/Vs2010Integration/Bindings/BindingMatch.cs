namespace TechTalk.SpecFlow.Bindings
{
    public class BindingMatchNew
    {
        static public readonly BindingMatchNew NonMatching = new BindingMatchNew(null, 0);

        public StepDefinitionBinding StepBinding { get; private set; }
        public int ScopeMatches { get; private set; }
        public bool Success { get { return StepBinding != null; } }

        public BindingMatchNew(StepDefinitionBinding stepBinding, int scopeMatches)
        {
            StepBinding = stepBinding;
            ScopeMatches = scopeMatches;
        }
    }
}