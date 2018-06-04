// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// This file has been modified by TechTalk.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;

namespace TechTalk.SpecFlow.Rpc.Server
{
    internal sealed class TcpClientConnectionHost : IClientConnectionHost
    {
        private TcpListener _listener;
        private int _connectionCount;

        public short Start(IPAddress ipAddress, Int16 startPort)
        {
            short usedPort = startPort;
            while (usedPort < short.MaxValue)
            {

                try
                {
                    _listener = new TcpListener(new IPEndPoint(ipAddress, usedPort));
                    _listener.Start();
                }
                catch (SocketException socketException) when (socketException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    usedPort++;
                    continue;
                }

                break;
            }

            return usedPort;
        }

        public async Task<IClientConnection> CreateListenTask(CancellationToken cancellationToken, Logger logger)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(true);
            tcpClient.ReceiveTimeout = tcpClient.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

            return new TcpClientConnection(tcpClient, _connectionCount++.ToString(), logger);
        }

        private sealed class TcpClientConnection : ClientConnection
        {
            private readonly TcpClient _client;

            internal TcpClientConnection(TcpClient client, string loggingIdentifier, Logger logger) : base(loggingIdentifier, client.GetStream(), logger)
            {
                _client = client;
            }

            public override void Close()
            {
                _client.Dispose();
            }

            protected override Task CreateMonitorDisconnectTask(CancellationToken cancellationToken)
            {
                return Task.Delay(-1, cancellationToken);
            }
        }
    }
}
