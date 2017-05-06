using System;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class InProcGeneratorInfoProvider : IGeneratorInfoProvider
    {
        public GeneratorInfo GetGeneratorInfo()
        {
            return new GeneratorInfo
                       {
                           GeneratorAssemblyVersion = typeof(InProcGeneratorInfoProvider).Assembly.GetName().Version,
                           GeneratorVersion = TestGeneratorFactory.GeneratorVersion,
                           GeneratorFolder = null
                       };
        }
    }
}