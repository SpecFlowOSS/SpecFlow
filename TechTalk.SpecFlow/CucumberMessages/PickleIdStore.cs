using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PickleIdStore : IPickleIdStore
    {
        private readonly IPickleIdGenerator _pickleIdGenerator;
        private readonly IDictionary<ScenarioInfo, Guid> _scenarioInfoMappings = new Dictionary<ScenarioInfo, Guid>();

        public PickleIdStore(IPickleIdGenerator pickleIdGenerator)
        {
            _pickleIdGenerator = pickleIdGenerator;
        }

        public Result GetPickleIdForScenario(ScenarioInfo scenarioInfo)
        {
            lock (_scenarioInfoMappings)
            {
                if (_scenarioInfoMappings.ContainsKey(scenarioInfo))
                {
                    return Result.Success(_scenarioInfoMappings[scenarioInfo]);
                }

                var pickleId = _pickleIdGenerator.GeneratePickleId();
                _scenarioInfoMappings.Add(scenarioInfo, pickleId);
                return Result.Success(pickleId);

            }
        }
    }
}
