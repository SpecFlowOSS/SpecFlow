using System;
using System.Collections.Generic;
using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class SpecFlowOutputHelper : ISpecFlowOutputHelper
    {
        private readonly IObjectContainer _container;

        public SpecFlowOutputHelper(IObjectContainer container)
        {
            _container = container;
        }

        private IEnumerable<ISpecFlowScenarioOutputListener> Listeners =>
            _container.ResolveAll<ISpecFlowScenarioOutputListener>();

        public void WriteLine(string message)
        {
            foreach (var listener in Listeners)
            {
                listener.OnMessage(message);
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
    }
}
