using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class ObsoleteStepHandler : IObsoleteStepHandler
    {
        private readonly IErrorProvider errorProvider;
        private readonly ITestTracer testTracer;
        protected readonly SpecFlowConfiguration specFlowConfiguration;

        public ObsoleteStepHandler(IErrorProvider errorProvider, ITestTracer testTracer, SpecFlowConfiguration specFlowConfiguration)
        {
            this.errorProvider = errorProvider;
            this.testTracer = testTracer;
            this.specFlowConfiguration = specFlowConfiguration;
        }

        public void Handle(BindingMatch bindingMatch)
        {
            if(bindingMatch.IsObsolete)
            {
                switch (specFlowConfiguration.ObsoleteBehavior)
                {
                    case ObsoleteBehavior.None:
                        break;
                    case ObsoleteBehavior.Warn:
                        testTracer.TraceWarning(bindingMatch.BindingObsoletion.Message);
                        break;
                    case ObsoleteBehavior.Pending:
                        throw errorProvider.GetPendingStepDefinitionError();
                    case ObsoleteBehavior.Error:
                        throw errorProvider.GetObsoleteStepError(bindingMatch.BindingObsoletion);
                }
            }
        }
    }
}