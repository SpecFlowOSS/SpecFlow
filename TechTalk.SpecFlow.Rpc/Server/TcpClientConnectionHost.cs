// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// This file has been modified by TechTalk.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Rpc.Server
{
    internal sealed class TcpClientConnectionHost : IClientConnectionHost
    {
        private readonly TcpListener _listener;
        private int _connectionCount;

        internal TcpClientConnectionHost(IPEndPoint endPoint)
        {
            _listener = new TcpListener(endPoint);
            _listener.Start();
        }

        public async Task<IClientConnection> CreateListenTask(CancellationToken cancellationToken)
        {
            try
            {
                var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(true);
                return new TcpClientConnection(tcpClient, _connectionCount++.ToString());
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        private sealed class TcpClientConnection : ClientConnection
        {
            private readonly TcpClient _client;

            internal TcpClientConnection(TcpClient client, string loggingIdentifier) : base(loggingIdentifier, client.GetStream())
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
