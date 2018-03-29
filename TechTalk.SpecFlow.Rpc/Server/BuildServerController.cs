// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Rpc.Shared;

namespace TechTalk.SpecFlow.Rpc.Server
{
    /// <summary>
    ///     Base type for the build server code.  Contains the basic logic for running the actual server, startup
    ///     and shutdown.
    /// </summary>
    public class BuildServerController
    {
        private readonly ObjectContainer _container;
        internal int DefaultPort = 4242;
        private ServerDispatcher _dispatcher;

        public BuildServerController(ObjectContainer container)
        {
            _container = container;
        }

        public int Run(int port)
        {
            DefaultPort = port;


            var pipeName = GetDefaultPipeName();
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => { cancellationTokenSource.Cancel(); };

            return RunServer(pipeName, cancellationToken: cancellationTokenSource.Token);
        }

        protected async Task<Stream> ConnectForShutdownAsync(string pipeName, int timeout)
        {
            var port = int.Parse(pipeName);
            var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port).ConfigureAwait(false);
            return client.GetStream();
        }

        private IClientConnectionHost CreateClientConnectionHost(string pipeName)
        {
            var port = int.Parse(pipeName);
            var endPoint = new IPEndPoint(IPAddress.Loopback, port);
            var connectionHost = new TcpClientConnectionHost(endPoint);
            return connectionHost;
        }

        private TimeSpan? GetKeepAliveTimeout()
        {
            return null;
        }

        private string GetDefaultPipeName()
        {
            return $"{DefaultPort}";
        }


        private int RunServer(string pipeName, IClientConnectionHost clientConnectionHost = null, IDiagnosticListener listener = null,
            TimeSpan? keepAlive = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            keepAlive = keepAlive ?? GetKeepAliveTimeout();
            listener = listener ?? new DiagnosticListener();
            clientConnectionHost = clientConnectionHost ?? CreateClientConnectionHost(pipeName);
            return RunServerCore(clientConnectionHost, listener, keepAlive, cancellationToken);
        }

        protected virtual int RunServerCore(IClientConnectionHost connectionHost, IDiagnosticListener listener, TimeSpan? keepAlive,
            CancellationToken cancellationToken)
        {
            CompilerServerLogger.Log("Keep alive timeout is: {0} milliseconds.", keepAlive?.TotalMilliseconds ?? 0);
            FatalError.Handler = FailFast.OnFatalException;

            _dispatcher = new ServerDispatcher(connectionHost, listener, _container);
            _dispatcher.ListenAndDispatchConnections(keepAlive, cancellationToken);
            return CommonCompiler.Succeeded;
        }

        public void Stop()
        {
            _dispatcher?.StopDispatching();
        }
    }
}