﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.PlatformSpecific;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void CanLoadConfigFromConfigFile()
        {
            var runtimeConfiguration = RuntimeConfigurationLoader.GetDefault();
            var configurationLoader = new RuntimeConfigurationLoader();

            runtimeConfiguration = configurationLoader.Load(runtimeConfiguration);
        }

        [Test]
        [TestCase(@"<specFlow>
    <language feature=""en"" tool=""en"" /> 

    <unitTestProvider name=""NUnit"" 
                      generatorProvider=""TechTalk.SpecFlow.TestFrameworkIntegration.NUnitRuntimeProvider, TechTalk.SpecFlow""
                      runtimeProvider=""TechTalk.SpecFlow.UnitTestProvider.NUnitRuntimeProvider, TechTalk.SpecFlow"" />

    <generator allowDebugGeneratedFiles=""false"" />
    
    <runtime detectAmbiguousMatches=""true""
             stopAtFirstError=""false""
             missingOrPendingStepsOutcome=""Inconclusive"" />

    <trace traceSuccessfulSteps=""true""
           traceTimings=""false""
           minTracedDuration=""0:0:0.1""
           listener=""TechTalk.SpecFlow.Tracing.DefaultListener, TechTalk.SpecFlow""
            />
</specFlow>")]
        public void CanLoadConfigFromString(string configString)
        {
            var runtimeConfig = RuntimeConfigurationLoader.GetDefault();

            var configurationLoader = new RuntimeConfigurationLoader();


            var configurationSectionHandler = ConfigurationSectionHandler.CreateFromXml(configString);
            configurationLoader.LoadAppConfig(runtimeConfig, configurationSectionHandler);
        }
    }
}
