namespace TechTalk.SpecFlow.Bindings
{
    public class BindingMatchNew
    {
        static public readonly BindingMatchNew NonMatching = new BindingMatchNew(null, 0);

        public StepBindingNew StepBinding { get; private set; }
        public int ScopeMatches { get; private set; }
        public bool Success { get { return StepBinding != null; } }

        public BindingMatchNew(StepBindingNew stepBinding, int scopeMatches)
        {
            StepBinding = stepBinding;
            ScopeMatches = scopeMatches;
        }
    }
}