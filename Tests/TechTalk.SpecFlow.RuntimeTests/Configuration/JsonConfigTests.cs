using System;
using System.Linq;
using BoDi;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.JsonConfig;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.RuntimeTests.Configuration
{
    [TestFixture]
    public class JsonConfigTests
    {            
        [Test]
        [TestCase(@"{
          ""specFlow"": {
            ""language"": {
              ""feature"": ""en"",
              ""tool"": ""en""
            },
            ""unitTestProvider"": {
              ""name"": ""NUnit"",
              ""generatorProvider"": ""TechTalk.SpecFlow.TestFrameworkIntegration.NUnitRuntimeProvider, TechTalk.SpecFlow"",
              ""runtimeProvider"": ""TechTalk.SpecFlow.UnitTestProvider.NUnitRuntimeProvider, TechTalk.SpecFlow""
            },
            ""generator"": { ""allowDebugGeneratedFiles"": ""false"" , ""markFeaturesParallelizable"": ""false"", 
                             ""skipParallelizableMarkerForTags"": [""mySpecialTag1"", ""mySpecialTag2""]},
            ""runtime"": {
              ""detectAmbiguousMatches"": ""true"",
              ""stopAtFirstError"": ""false"",
              ""missingOrPendingStepsOutcome"": ""Inconclusive""
            },
            ""trace"": {
              ""traceSuccessfulSteps"": ""true"",
              ""traceTimings"": ""false"",
              ""minTracedDuration"": ""0:0:0.1"",
              ""listener"": ""TechTalk.SpecFlow.Tracing.DefaultListener, TechTalk.SpecFlow""
            }
          }
        }")]
        public void CanLoadConfigFromString(string configString)
        {
            var configurationLoader = new JsonConfigurationLoader();
            
            configurationLoader.LoadJson(ConfigurationLoader.GetDefault(), configString);
        }

        [Test]
        public void CheckFeatureLanguage()
        {
            string config = @"{
                              ""specflow"": {
                                ""language"": { ""feature"": ""de"" }
                              }
                            }";
            
            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.FeatureLanguage.TwoLetterISOLanguageName.Should().Be("de");
        }       

        [Test]
        public void CheckBindingCulture()
        {
            string config = @"{
                              ""specflow"": {
                                ""bindingCulture"": { ""name"": ""de"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.BindingCulture.TwoLetterISOLanguageName.Should().Be("de");
        }

        [Test]
        public void CheckUnitTestProvider()
        {
            string config = @"{
                              ""specflow"": {
                                ""unitTestProvider"": { ""name"": ""XUnit"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.UnitTestProvider.Should().Be("XUnit");
        }
        

        [Test]
        public void Check_Runtime_stopAtFirstError_as_true()
        {
            string config = @"{
                              ""specflow"": {
                                ""runtime"": { ""stopAtFirstError"": ""true"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.StopAtFirstError.Should().BeTrue();
        }

        [Test]
        public void Check_Runtime_stopAtFirstError_as_false()
        {
            string config = @"{
                              ""specflow"": {
                                ""runtime"": { ""stopAtFirstError"": ""false"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.StopAtFirstError.Should().BeFalse();
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Pending()
        {
            string config = @"{
                              ""specflow"": {
                                ""runtime"": { ""missingOrPendingStepsOutcome"": ""Pending"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Pending);
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Error()
        {
            string config = @"{
                                ""specflow"": {
                                ""runtime"": { ""missingOrPendingStepsOutcome"": ""Error"" }
                                }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Error);
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Ignore()
        {
            string config = @"{
                              ""specflow"": {
                                ""runtime"": { ""missingOrPendingStepsOutcome"": ""Ignore"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Ignore);
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Inconclusive()
        {
            string config = @"{
                              ""specflow"": {
                                ""runtime"": { ""missingOrPendingStepsOutcome"": ""Inconclusive"" }
                              }
                            }";



            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Inconclusive);
        }

        [Test]
        public void Check_Trace_traceSuccessfulSteps_as_True()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""traceSuccessfulSteps"": ""true"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.TraceSuccessfulSteps.Should().BeTrue();
        }

        [Test]
        public void Check_Trace_traceSuccessfulSteps_as_False()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""traceSuccessfulSteps"": ""false"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.TraceSuccessfulSteps.Should().BeFalse();
        }

        [Test]
        public void Check_Trace_traceTimings_as_True()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""traceTimings"": ""true"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.TraceTimings.Should().BeTrue();
        }

        [Test]
        public void Check_Trace_traceTimings_as_False()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""traceTimings"": ""false"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.TraceTimings.Should().BeFalse();
        }

        [Test]
        public void Check_Trace_minTracedDuration()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""minTracedDuration"": ""0:0:1.0"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.MinTracedDuration.Should().Be(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void Trace_Listener_Not_Supported()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""listener"": ""TraceListener"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.CustomDependencies.Count.Should().Be(0);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_RegexAttribute()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""stepDefinitionSkeletonStyle"": ""RegexAttribute"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.RegexAttribute);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNamePascalCase()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""stepDefinitionSkeletonStyle"": ""MethodNamePascalCase"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNamePascalCase);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNameRegex()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""stepDefinitionSkeletonStyle"": ""MethodNameRegex"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNameRegex);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNameUnderscores()
        {
            string config = @"{
                              ""specflow"": {
                                ""trace"": { ""stepDefinitionSkeletonStyle"": ""MethodNameUnderscores"" }
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNameUnderscores);
        }

        [Test]
        public void Check_StepAssemblies_IsEmpty()
        {
            string config = @"{
                              ""specflow"": {
                                  ""stepAssemblies"" : [
                                 ]
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.AdditionalStepAssemblies.Should().BeEmpty();
        }

        [Test]
        public void Check_StepAssemblies_NotInConfigFile()
        {
            string config = @"{
                                ""specflow"": {
    
                                }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.AdditionalStepAssemblies.Should().BeEmpty();
        }

        [Test]
        public void Check_StepAssemblies_OneEntry()
        {
            string config = @"{
                              ""specflow"": {
                                ""stepAssemblies"": 
                                   [ {""assembly"": ""testEntry""} ]
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.AdditionalStepAssemblies.Count.Should().Be(1);
            runtimeConfig.AdditionalStepAssemblies.First().Should().Be("testEntry");
        }

        [Test]
        public void Check_StepAssemblies_TwoEntry()
        {
            string config = @"{
                              ""specflow"": {
                                ""stepAssemblies"": [
                                    { ""assembly"": ""testEntry1"" },
                                    { ""assembly"": ""testEntry2"" }
                                  ]
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.AdditionalStepAssemblies.Count.Should().Be(2);
            runtimeConfig.AdditionalStepAssemblies[0].Should().Be("testEntry1");
            runtimeConfig.AdditionalStepAssemblies[1].Should().Be("testEntry2");
        }

        [Test]
        public void Check_Plugins_IsEmpty()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": []
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Should().BeEmpty();
        }

        [Test]
        public void Check_Plugins_NotInConfigFile()
        {
            string config = @"{
                              ""specflow"": {
    
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Should().BeEmpty();
        }

        [Test]
        public void Check_Plugins_OneEntry()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry"" }
                                  ]                                
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Name.Should().Be("testEntry");
        }

        [Test]
        public void Check_Plugins_PluginPath()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry"", ""path"":""path_to_assembly"" }
                                  ]                                
                              }
                            }";
            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Path.Should().Be("path_to_assembly");
        }

        [Test]
        public void Check_Plugins_Parameters()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry"", ""parameters"":""pluginParameter"" }
                                  ]                                
                              }
                            }";
            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Parameters.Should().Be("pluginParameter");
        }

        [Test]
        public void Check_Plugins_PluginType_Runtime()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry"", ""type"":""Runtime"" }
                                  ]                                
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Type.Should().Be(PluginType.Runtime);
        }

        [Test]
        public void Check_Plugins_PluginType_Generator()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry"", ""type"":""Generator"" }
                                  ]                                
                              }
                            }";
            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Type.Should().Be(PluginType.Generator);
        }

        [Test]
        public void Check_Plugins_PluginType_GeneratorAndRuntime()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry"", ""type"":""GeneratorAndRuntime"" }
                                  ]                                
                              }
                            }";        
            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Type.Should().Be(PluginType.GeneratorAndRuntime);
        }

        [Test]
        public void Check_Plugins_TwoEntry()
        {
            string config = @"{
                              ""specflow"": {
                                ""plugins"": 
                                  [
                                    { ""name"": ""testEntry1"" },
                                    { ""name"": ""testEntry2"" }
                                  ]                                
                              }
                            }";

            

            var runtimeConfig = new JsonConfigurationLoader().LoadJson(ConfigurationLoader.GetDefault(), config);

            runtimeConfig.Plugins.Count.Should().Be(2);
            runtimeConfig.Plugins[0].Name.Should().Be("testEntry1");
            runtimeConfig.Plugins[1].Name.Should().Be("testEntry2");
        }
    }
}
