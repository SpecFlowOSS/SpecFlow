using System.IO;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class FeatureCodeBehindGenerator : IFeatureCodeBehindGenerator
    {
        private readonly BuildServerController _buildServerController;

        public FeatureCodeBehindGenerator(BuildServerController buildServerController)
        {
            _buildServerController = buildServerController;
        }

        public void InitializeProject(string projectPath)
        {
            
        }

        public GeneratedCodeBehindFile GenerateCodeBehindFile(string featureFile)
        {
            return new GeneratedCodeBehindFile()
            {
                Filename = Path.GetFileName(featureFile) + ".cs",
                Content = "from another process with love!"
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