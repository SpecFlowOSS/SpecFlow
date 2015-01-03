namespace TechTalk.SpecFlow.Bindings
{
	public class StepBindingMatch : BindingMatch
	{
		static public readonly StepBindingMatch NonMatching = new StepBindingMatch(null, 0, null, null);
		public IStepDefinitionBinding StepBinding { get { return (IStepDefinitionBinding)Binding; }}

		public StepBindingMatch(IStepDefinitionBinding stepBinding, int scopeMatches, object[] arguments, StepContext stepContext) 
			: base(stepBinding, scopeMatches, arguments, stepContext)
		{
		}
	}

	public class BindingMatch
    {
		public IRegexBinding Binding { get; private set; }
        public bool Success { get { return Binding != null; } }
        
        public int ScopeMatches { get; private set; }
        public bool IsScoped { get { return ScopeMatches > 0; } }

        public object[] Arguments  { get; private set; }
        public StepContext StepContext { get; private set; }

        public BindingMatch(IRegexBinding stepBinding, int scopeMatches, object[] arguments, StepContext stepContext)
        {
            Binding = stepBinding;
            ScopeMatches = scopeMatches;
            Arguments = arguments;
            StepContext = stepContext;
        }
    }
}