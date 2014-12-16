using System;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

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
        private readonly IVisualStudioTracer visualStudioTracer;

        public GherkinScopeAnalyzer(IProjectScope projectScope, IVisualStudioTracer visualStudioTracer)
        {
            this.projectScope = projectScope;
            this.visualStudioTracer = visualStudioTracer;
        }

        public GherkinFileScopeChange Analyze(GherkinFileScopeChange change)
        {
            visualStudioTracer.Trace("Analyzing started", "GherkinScopeAnalyzer");
            return change; //TODO
        }
    }
}