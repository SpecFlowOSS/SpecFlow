using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PickleIdStoreDictionaryFactory : IPickleIdStoreDictionaryFactory
    {
        public IDictionary<ScenarioInfo, Guid> BuildDictionary()
        {
            return new ConcurrentDictionary<ScenarioInfo, Guid>();
        }
    }
}
