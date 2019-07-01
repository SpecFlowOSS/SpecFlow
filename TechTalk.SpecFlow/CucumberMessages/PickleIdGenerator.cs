using System;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PickleIdGenerator : IPickleIdGenerator
    {
        public Guid GeneratePickleId()
        {
            return Guid.NewGuid();
        }
    }
}
