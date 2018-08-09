using System.Reflection;

using TechTalk.SpecFlow.MsBuildNetSdk.IntegrationTests.Features;
using Xunit;

namespace TechTalk.SpecFlow.MsBuildNetSdk.IntegrationTests
{
    
    public class CodeBehindFileGenerationTests
    {
        [Fact]
        public void TestIfCodeBehindFilesWasGeneratedAndCompiled()
        {
            var assemblyHoldingThisClass = Assembly.GetExecutingAssembly();
            var typeOfGeneratedFeatureFile = assemblyHoldingThisClass.GetType(typeof(DummyFeatureFileToTestMSBuildNetsdkCodebehindFileGenerationFeature).FullName);
            Assert.NotNull(typeOfGeneratedFeatureFile);
        }
    }
}
