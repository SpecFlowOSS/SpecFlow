using System;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using FluentAssertions;
using Serilog;
using Serilog.Core;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Rpc.Server;
using TechTalk.SpecFlow.Rpc.Shared;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{
    public class SameProcess : IDisposable
    {
        public SameProcess()
        {
            _container = new ObjectContainer();

            _featureCodeBehindGeneratorMock = new TestServerInterface();

            _container.RegisterInstanceAs<ITestServerInterface>(_featureCodeBehindGeneratorMock);

            _freePort = FindFreePort.GetAvailablePort(4635);
        }

        public void Dispose()
        {
            _buildServerController?.Stop();
        }

        private BuildServerController _buildServerController;
        private readonly ObjectContainer _container;
        private readonly TestServerInterface _featureCodeBehindGeneratorMock;
        private bool _serverStarted;
        private Exception _serverStartupException;
        private readonly int _freePort;


        private void Start()
        {
            try
            {
                var logger = new LoggerConfiguration().CreateLogger();


                _serverStarted = true;
                _buildServerController = new BuildServerController(_container, logger);
                _buildServerController.Run(_freePort);
            }
            catch (Exception e)
            {
                _serverStartupException = e;
            }
        }

        private void EnsureServerStarted()
        {
            using (var client = new Client<ITestServerInterface>(_freePort))
            {
                var task = client.WaitForServer();

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10));

                if (!isCompletedSuccessfully)
                {
                    throw new InvalidOperationException("Server startup exception", _serverStartupException);
                }
            }
        }


        [Fact]
        public async Task ComplexClient_WaitForServer()
        {
            var thread = new Thread(Start);
            thread.Start();

            using (var client = new Client<ITestServerInterface>(_freePort))
            {
                await client.WaitForServer();
            }

            _serverStarted.Should().BeTrue();
        }

        [Fact]
        public async Task ComplexClient_SingleCall()
        {
            var thread = new Thread(Start);
            thread.Start();
            EnsureServerStarted();

            using (var client = new Client<ITestServerInterface>(_freePort))
            {
                var result = await client.Execute(c => c.MethodWithParameter("FeatureFilePath")).ConfigureAwait(false);

                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task ComplexClient_MultipleCalls()
        {
            var thread = new Thread(Start);
            thread.Start();
            EnsureServerStarted();

            using (var client = new Client<ITestServerInterface>(_freePort))
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
            var thread = new Thread(Start);
            thread.Start();
            EnsureServerStarted();

            using (var client = new Client<ITestServerInterface>(_freePort))
            {
                await client.ShutdownServer();
            }
        }
    }
}