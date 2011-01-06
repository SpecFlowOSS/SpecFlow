using System;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public enum BindingStatusKind
    {
        Unknown,
        Unbound,
        Valid,
        Invalid
    }

    public class BindingStatus
    {
        public readonly static BindingStatus UnknownBindingStatus = new BindingStatus(BindingStatusKind.Unknown);
        public readonly static BindingStatus UnboundBindingStatus = new BindingStatus(BindingStatusKind.Unbound);

        public BindingStatusKind Kind { get; private set; }

        public BindingStatus(BindingStatusKind kind)
        {
            Kind = kind;
        }
    }

    public class GherkinScopeAnalyzer
    {
        private IProjectScope projectScope;

        public GherkinScopeAnalyzer(IProjectScope projectScope)
        {
            this.projectScope = projectScope;
        }

        public GherkinFileScopeChange Analyze(GherkinFileScopeChange change, IGherkinFileScope previousScope = null)
        {
            throw new NotImplementedException();
        }
    }
}