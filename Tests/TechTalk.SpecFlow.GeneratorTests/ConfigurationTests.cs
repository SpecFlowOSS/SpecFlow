using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private const string ConfigWithParallelCodeGenerationOptions =
            @"<specFlow>
                <language feature=""en"" tool=""en"" /> 

                <unitTestProvider name=""NUnit"" 
                                    generatorProvider=""TechTalk.SpecFlow.TestFrameworkIntegration.NUnitRuntimeProvider, TechTalk.SpecFlow""
                                    runtimeProvider=""TechTalk.SpecFlow.UnitTestProvider.NUnitRuntimeProvider, TechTalk.SpecFlow"" />

                <generator allowDebugGeneratedFiles=""false""
                           markFeaturesParallelizable=""true"">
                    <skipParallelizableMarkerForTags>
                        <tag value=""mySpecialTag1""/>
                        <tag value=""mySpecialTag2""/>
                    </skipParallelizableMarkerForTags>
                </generator>
                 
    
                <runtime detectAmbiguousMatches=""true""
                            stopAtFirstError=""false""
                            missingOrPendingStepsOutcome=""Inconclusive"" />

                <trace traceSuccessfulSteps=""true""
                        traceTimings=""false""
                        minTracedDuration=""0:0:0.1""
                        listener=""TechTalk.SpecFlow.Tracing.DefaultListener, TechTalk.SpecFlow""
                        />
            </specFlow>";

        [Test]
        [TestCase(ConfigWithParallelCodeGenerationOptions, Description = "Config with Parallel Code Generation Options")]
        public void CanLoadConfigWithParallelCodeGenerationOptionsFromString(string configString)
        {
            var generatorConfiguration = new GeneratorConfiguration();
            generatorConfiguration.UpdateFromConfigFile(ConfigurationSectionHandler.CreateFromXml(configString));

            Assert.IsTrue(generatorConfiguration.MarkFeaturesParallelizable);
            Assert.IsNotEmpty(generatorConfiguration.SkipParallelizableMarkerForTags);
            Assert.Contains("mySpecialTag1",generatorConfiguration.SkipParallelizableMarkerForTags);
            Assert.Contains("mySpecialTag2", generatorConfiguration.SkipParallelizableMarkerForTags);
        }
    }
}