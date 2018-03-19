using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;

namespace TechTalk.SpecFlow.Rpc.Client
{
    public class RawClient : IDisposable
    {
        private readonly int _port;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        public RawClient(int port)
        {
            _port = port;
        }

        public async Task<TResponse> SendRequest<TResponse>(Request buildRequest) where TResponse : Response
        {
            await EnsureConnection().ConfigureAwait(false);

            RequestStreamHandler.Write(buildRequest, _networkStream);

            //Thread.Sleep(10000);

            return ResponseStreamHandler.Read<TResponse>(_networkStream);

        }

        private async Task EnsureConnection()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
            }

            if (_tcpClient.Connected == false)
            {
                await _tcpClient.ConnectAsync(IPAddress.Loopback, _port).ConfigureAwait(false);
            }

            if (_networkStream == null)
            {
                _networkStream = _tcpClient.GetStream();
            }
        }

        public void Dispose()
        {
            _networkStream?.Close();
            _networkStream?.Dispose();
            _networkStream = null;

            _tcpClient?.Close();
            _tcpClient?.Dispose();
            _tcpClient = null;
        }
    }
}

