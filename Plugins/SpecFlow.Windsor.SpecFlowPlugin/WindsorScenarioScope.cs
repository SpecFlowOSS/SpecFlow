using System;

namespace SpecFlow.Windsor
{
    internal class WindsorScenarioScope : IDisposable
    {
        private readonly IDisposable scope;

        public WindsorScenarioScope(IDisposable scope)
        {
            this.scope = scope;
        }

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}
