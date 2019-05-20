using System;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class PickleIdGenerator : IPickleIdGenerator
    {
        public Result GeneratePickleId()
        {
            return Result.Success(Guid.NewGuid());
        }
    }
}
