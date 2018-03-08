﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.CodeBehindGenerator.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Client
{
    public class Client
    {
        private readonly int _port;

        public Client(int port)
        {
            _port = port;
        }

        public async Task<BuildResponse> SendRequest(BuildRequest buildRequest)
        {
            using (var tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(IPAddress.Loopback, _port);
                using (var networkStream = tcpClient.GetStream())
                {
                    await buildRequest.WriteAsync(networkStream);

                    var buildResponse = await BuildResponse.ReadAsync(networkStream);

                    return buildResponse;
                }
            }
        }
    }
}
