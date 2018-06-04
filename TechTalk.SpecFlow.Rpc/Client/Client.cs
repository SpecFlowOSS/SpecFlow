using System;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading.Tasks;

using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;
using Utf8Json;

namespace TechTalk.SpecFlow.Rpc.Client
{
    public class Client<TClientInterface> : IDisposable
    {
        private readonly int _port;

        //private RawClient _rawClient;
        private readonly ExtractMethodInfo<TClientInterface> _extractMethodInfo;

        public Client(int port)
        {
            _port = port;

            _extractMethodInfo = new ExtractMethodInfo<TClientInterface>();
        }

        public void Dispose()
        {
            //_rawClient?.Dispose();
            //_rawClient = null;
        }

        public async Task<TResult> Execute<TResult>(Expression<Func<TClientInterface, TResult>> p)
        {
            var methodInfo = _extractMethodInfo.ExtractFunction(p);
            var request = CreateRequest(methodInfo);

            Response response;
            using (var rawClient = new RawClient(_port))
            {
                response = await rawClient.SendRequest<Response>(request).ConfigureAwait(false);
            }

            try
            {
                var result = JsonSerializer.Deserialize<TResult>(response.Result, SerializationOptions.Current);

                return result;
            }
            catch (Utf8Json.JsonParsingException jsonReaderException)
            {
                throw new Exception($"Error while deserializing result: {response.Result}", jsonReaderException);

            }
        }

        public async Task Execute(Expression<Action<TClientInterface>> p)
        {
            var methodInfo = _extractMethodInfo.ExtractMethod(p);
            var request = CreateRequest(methodInfo);

            using (var rawClient = new RawClient(_port))
            {
                await rawClient.SendRequest<Response>(request).ConfigureAwait(false);
            }
        }

        private Request CreateRequest(InterfaceMethodInfo methodInfo)
        {
            var request = new Request
            {
                Assembly = methodInfo.Assembly,
                Type = methodInfo.Typename,
                Method = methodInfo.Methodname,
                Arguments = JsonSerializer.Serialize(methodInfo.Arguments, SerializationOptions.Current).ToString()
            };
            return request;
        }

        public async Task ShutdownServer()
        {
            using (var rawClient = new RawClient(_port))
            {
                await rawClient.SendRequest<Response>(new Request() { IsShutDown = true }).ConfigureAwait(false);
            }
        }
        public async Task Ping()
        {
            using (var rawClient = new RawClient(_port))
            {
                await rawClient.SendRequest<Response>(new Request() { IsPing = true }).ConfigureAwait(false);
            }
        }

        public async Task WaitForServer()
        {
            while (true)
            {
                try
                {
                    await Ping();
                    break;
                }
                catch (SocketException socketException)
                {
                    //todo logging
                }
            }
        }
    }
}