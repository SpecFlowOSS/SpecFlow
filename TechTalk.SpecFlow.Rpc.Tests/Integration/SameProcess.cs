using System;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Rpc.Server;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{
    public class SameProcess : IDisposable
    {
        private BuildServerController _buildServerController;

        //[Fact]
        //public async Task SendRequestGetResoonse()
        //{
        //    int port = 4635;

        //    var thread = new Thread(Start);
        //    thread.Start();

        //    Thread.Sleep(1000);

        //    var buildRequest = new InitProjectRequest();

        //    using (var client = new RawClient(port))
        //    {

        //        var response = await client.SendRequest<InitProjectResponse>(buildRequest).ConfigureAwait(false);

        //        Assert.NotNull(response);
        //    }
        //}

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
            _buildServerController = new BuildServerController();
            _buildServerController.Run(4635);
        }

        public void Dispose()
        {
            //_buildServerController?.Stop();
        }
    }
}
