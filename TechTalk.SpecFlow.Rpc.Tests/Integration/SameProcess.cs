using System;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.CodeBehindGenerator;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Rpc.Server;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{
    public class SameProcess : IDisposable
    {
        public SameProcess()
        {
            _container = new ObjectContainer();

            _featureCodeBehindGeneratorMock = new FeatureCodeBehindGeneratorMock(_buildServerController);

            _container.RegisterInstanceAs<IFeatureCodeBehindGenerator>(_featureCodeBehindGeneratorMock);
        }

        public void Dispose()
        {
            _buildServerController?.Stop();
        }

        private BuildServerController _buildServerController;
        private readonly ObjectContainer _container;
        private readonly FeatureCodeBehindGeneratorMock _featureCodeBehindGeneratorMock;


        private void Start()
        {
            _buildServerController = new BuildServerController(_container);
            _buildServerController.Run(4635);
        }


        [Fact]
        public async Task ComplexClient_SingleCall()
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

        [Fact]
        public async Task ComplexClient_MultipleCalls()
        {
            int port = 4635;

            var thread = new Thread(Start);
            thread.Start();

            Thread.Sleep(1000);

            using (var client = new Client<IFeatureCodeBehindGenerator>(port))
            {
                for (int i = 0; i < 5; i++)
                {
                    var result = await client.Execute(c => c.GenerateCodeBehindFile("FeatureFilePath" + i)).ConfigureAwait(false);
                    Assert.NotNull(result);
                }
            }
        }

        [Fact]
        public async Task ComplexClient_Shutdown()
        {
            int port = 4635;

            var thread = new Thread(Start);
            thread.Start();

            Thread.Sleep(1000);

            using (var client = new Client<IFeatureCodeBehindGenerator>(port))
            {
                await client.ShutdownServer();
            }
        }
    }
}