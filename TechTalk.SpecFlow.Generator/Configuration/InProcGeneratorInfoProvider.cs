using System;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class InProcGeneratorInfoProvider : IGeneratorInfoProvider
    {
        private readonly GeneratorConfiguration generatorConfiguration;

        public InProcGeneratorInfoProvider(GeneratorConfiguration generatorConfiguration)
        {
            this.generatorConfiguration = generatorConfiguration;
        }

        public GeneratorInfo GetGeneratorInfo()
        {
            return new GeneratorInfo
                       {
                           GeneratorAssemblyVersion = typeof(InProcGeneratorInfoProvider).Assembly.GetName().Version,
                           GeneratorVersion = TestGeneratorFactory.GeneratorVersion,
                           GeneratorFolder = null,
                           UsesPlugins = generatorConfiguration.UsesPlugins
                       };
        }
    }
}