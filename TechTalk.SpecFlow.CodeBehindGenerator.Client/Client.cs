using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Client
{
    public class Client : IDisposable
    {
        private readonly int _port;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        public Client(int port)
        {
            _port = port;
        }

        public async Task<TResponse> SendRequest<TResponse>(BaseRequest buildRequest) where TResponse : BaseResponse
        {
            await EnsureConnection();

            RequestStream.Write(buildRequest, _networkStream);

            return ResponseStream.Read<TResponse>(_networkStream);

        }

        private async Task EnsureConnection()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
            }

            if (_tcpClient.Connected == false)
            {
                await _tcpClient.ConnectAsync(IPAddress.Loopback, _port);
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

