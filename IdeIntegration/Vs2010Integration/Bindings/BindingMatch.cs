namespace TechTalk.SpecFlow.Bindings
{
    public class BindingMatch
    {
        static public readonly BindingMatch NonMatching = new BindingMatch(null, 0);

        public StepBinding StepBinding { get; private set; }
        public int ScopeMatches { get; private set; }
        public bool Success { get { return StepBinding != null; } }

        public BindingMatch(StepBinding stepBinding, int scopeMatches)
        {
            StepBinding = stepBinding;
            ScopeMatches = scopeMatches;
        }
    }
}