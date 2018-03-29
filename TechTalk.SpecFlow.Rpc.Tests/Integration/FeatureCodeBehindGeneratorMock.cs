using TechTalk.SpecFlow.CodeBehindGenerator;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{
    class FeatureCodeBehindGeneratorMock : IFeatureCodeBehindGenerator
    {
        private BuildServerController _buildServerController;

        public FeatureCodeBehindGeneratorMock(BuildServerController buildServerController)
        {
            _buildServerController = buildServerController;
        }

        public void InitializeProject(string projectPath)
        {
        }

        GeneratedCodeBehindFile IFeatureCodeBehindGenerator.GenerateCodeBehindFile(string featureFile)
        {
            return new GeneratedCodeBehindFile()
            {
                Content = "codeBehindFileCode",
                Filename = "Feature.feature.cs"
            };
        }

        public void Ping()
        {
        }

        public void Shutdown()
        {
            _buildServerController.Stop();
        }
    }
}