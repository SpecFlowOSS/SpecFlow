using System;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Rpc.Server;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{

    class FeatureCodeBehindGeneratorMock : IFeatureCodeBehindGenerator
    {
        public void InitializeProject(string projectPath)
        {
            
        }

        public string GenerateCodeBehindFile(string featureFile)
        {
            return "codeBehindFileCode";
        }
    }

    public class SameProcess : IDisposable
    {
        private BuildServerController _buildServerController;
        private ObjectContainer _container;

        public SameProcess()
        {
            _container = new ObjectContainer();
            _container.RegisterTypeAs<FeatureCodeBehindGeneratorMock, IFeatureCodeBehindGenerator>();
        }


        [Fact]
        public async Task ComplexClient()
        {
            int port = 4635;

            var thread = new Thread(Start);
            thread.Start();

            Thread.Sleep(1000);

            using (var client = new Client<IFeatureCodeBehindGenerator>(port))
            {
                var result = await client.Execute(c => c.GenerateCodeBehindFile("FeatureFilePath")).ConfigureAwait(false);

                Assert.NotNull(result);
            }
        }


        private void Start()
        {
            _buildServerController = new BuildServerController(_container);
            _buildServerController.Run(4635);
        }

        public void Dispose()
        {
            //_buildServerController?.Stop();
        }
    }
}
