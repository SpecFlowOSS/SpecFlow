using System;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Rpc.Server;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{

    public interface ITestServerInterface
    {
        string MethodWithParameter(string parameter);
    }

    public class TestServerInterface : ITestServerInterface
    {
        public string MethodWithParameter(string parameter)
        {
            return parameter;
        }
    }

    public class SameProcess : IDisposable
    {
        public SameProcess()
        {
            _container = new ObjectContainer();

            _featureCodeBehindGeneratorMock = new TestServerInterface();

            _container.RegisterInstanceAs<ITestServerInterface>(_featureCodeBehindGeneratorMock);
        }

        public void Dispose()
        {
            _buildServerController?.Stop();
        }

        private BuildServerController _buildServerController;
        private readonly ObjectContainer _container;
        private readonly TestServerInterface _featureCodeBehindGeneratorMock;


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

            using (var client = new Client<ITestServerInterface>(port))
            {
                var result = await client.Execute(c => c.MethodWithParameter("FeatureFilePath")).ConfigureAwait(false);

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

            using (var client = new Client<ITestServerInterface>(port))
            {
                for (int i = 0; i < 5; i++)
                {
                    var result = await client.Execute(c => c.MethodWithParameter("FeatureFilePath" + i)).ConfigureAwait(false);
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

            using (var client = new Client<ITestServerInterface>(port))
            {
                await client.ShutdownServer();
            }
        }
    }
}