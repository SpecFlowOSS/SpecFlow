using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PickleIdStore : IPickleIdStore
    {
        private readonly IPickleIdGenerator _pickleIdGenerator;
        private readonly IPickleIdStoreDictionaryFactory _pickleIdStoreDictionaryFactory;
        private readonly object _initializationLock = new object();
        private IDictionary<ScenarioInfo, Guid> _scenarioInfoMappings;

        public PickleIdStore(IPickleIdGenerator pickleIdGenerator, IPickleIdStoreDictionaryFactory pickleIdStoreDictionaryFactory)
        {
            _pickleIdGenerator = pickleIdGenerator;
            _pickleIdStoreDictionaryFactory = pickleIdStoreDictionaryFactory;
        }

        public Guid GetPickleIdForScenario(ScenarioInfo scenarioInfo)
        {
            EnsureIsInitialized();

            if (_scenarioInfoMappings.ContainsKey(scenarioInfo))
            {
                return _scenarioInfoMappings[scenarioInfo];
            }

            var pickleId = _pickleIdGenerator.GeneratePickleId();
            _scenarioInfoMappings.Add(scenarioInfo, pickleId);
            return pickleId;
        }

        public void EnsureIsInitialized()
        {
            if (_scenarioInfoMappings != null)
            {
                return;
            }

            lock (_initializationLock)
            {
                if (_scenarioInfoMappings != null)
                {
                    return;
                }

                _scenarioInfoMappings = _pickleIdStoreDictionaryFactory.BuildDictionary();
            }
        }
    }
}
