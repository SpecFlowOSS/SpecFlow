using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.CodeBehindGenerator.Client;
using TechTalk.SpecFlow.CodeBehindGenerator.Server;
using Xunit;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Tests.Integration
{
    public class SameProcess : IDisposable
    {
        private BuildServerController _buildServerController;

        [Fact]
        public async Task SendRequestGetResoonse()
        {
            var port = 4635;

            var thread = new Thread(Start);
            thread.Start();

            Thread.Sleep(1000);

            var buildRequest = BuildRequest.Create(Environment.CurrentDirectory, Path.GetTempPath(), new List<string>(){ "ToServer"});

            var client = new Client.Client(port);

            var response = await client.SendRequest(buildRequest);

            Assert.NotNull(response);
        }

        private void Start()
        {
             _buildServerController = new BuildServerController();
            _buildServerController.Run(4635);
        }

        public void Dispose()
        {
            _buildServerController?.Stop();
        }
    }
}
