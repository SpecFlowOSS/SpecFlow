using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IPickleIdStoreDictionaryFactory
    {
        IDictionary<ScenarioInfo, Guid> BuildDictionary();
    }
}
