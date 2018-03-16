using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.CodeBehindGenerator.Client;
using TechTalk.SpecFlow.CodeBehindGenerator.Server;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response;
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

            var buildRequest = new InitProjectRequest();

            using (var client = new Client.RawClient(port))
            {

                var response = await client.SendRequest<InitProjectResponse>(buildRequest).ConfigureAwait(false);

                Assert.NotNull(response);
            }
        }

        [Fact]
        public async Task ComplexClient()
        {
            var port = 4635;

            var thread = new Thread(Start);
            thread.Start();

            Thread.Sleep(1000);

            using (var client = new Client<IFeatureCodeBehindGenerator>())
            {
                var result = await client.Execute(c => c.GenerateCodeBehindFile("FeatureFilePath")).ConfigureAwait(false);

                Assert.NotNull(result);
            }
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
