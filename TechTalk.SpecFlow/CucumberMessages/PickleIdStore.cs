using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PickleIdStore : IPickleIdStore
    {
        private readonly IPickleIdGenerator _pickleIdGenerator;
        private readonly IDictionary<ScenarioInfo, Guid> _scenarioInfoMappings;

        public PickleIdStore(IPickleIdGenerator pickleIdGenerator, IDictionary<ScenarioInfo, Guid> scenarioInfoMappings)
        {
            _pickleIdGenerator = pickleIdGenerator;
            _scenarioInfoMappings = scenarioInfoMappings;
        }

        public Guid GetPickleIdForScenario(ScenarioInfo scenarioInfo)
        {
            lock (_scenarioInfoMappings)
            {
                if (_scenarioInfoMappings.ContainsKey(scenarioInfo))
                {
                    return _scenarioInfoMappings[scenarioInfo];
                }

                var pickleId = _pickleIdGenerator.GeneratePickleId();
                _scenarioInfoMappings.Add(scenarioInfo, pickleId);
                return pickleId;
            }
        }
    }
}
