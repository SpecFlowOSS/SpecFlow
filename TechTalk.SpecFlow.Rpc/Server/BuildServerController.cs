// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;

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

        public BuildServerController(ObjectContainer container)
        {
            _container = container;
        }

        public int Run(int port)
        {
            DefaultPort = port;
            var shutdown = false;


            var pipeName = GetDefaultPipeName();
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => { cancellationTokenSource.Cancel(); };

            return shutdown
                ? RunShutdown(pipeName, cancellationToken: cancellationTokenSource.Token)
                : RunServer(pipeName, cancellationToken: cancellationTokenSource.Token);
        }

        protected async Task<Stream> ConnectForShutdownAsync(string pipeName, int timeout)
        {
            var port = int.Parse(pipeName);
            var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port).ConfigureAwait(false);
            return client.GetStream();
        }

        protected IClientConnectionHost CreateClientConnectionHost(string pipeName)
        {
            var port = int.Parse(pipeName);
            var endPoint = new IPEndPoint(IPAddress.Loopback, port);
            var connectionHost = new TcpClientConnectionHost(endPoint);
            return connectionHost;
        }

        protected internal TimeSpan? GetKeepAliveTimeout()
        {
            return null;
        }

        protected string GetDefaultPipeName()
        {
            return $"{DefaultPort}";
        }


        /// <summary>
        ///     Was a server running with the specified session key during the execution of this call?
        /// </summary>
        protected virtual bool? WasServerRunning(string pipeName)
        {
            return null;
        }

        protected virtual int RunServerCore(string pipeName, IClientConnectionHost connectionHost, IDiagnosticListener listener, TimeSpan? keepAlive,
            CancellationToken cancellationToken)
        {
            CompilerServerLogger.Log("Keep alive timeout is: {0} milliseconds.", keepAlive?.TotalMilliseconds ?? 0);
            FatalError.Handler = FailFast.OnFatalException;

            var dispatcher = new ServerDispatcher(connectionHost, listener);
            dispatcher.ListenAndDispatchConnections(keepAlive, cancellationToken);
            return CommonCompiler.Succeeded;
        }

        internal int RunServer(string pipeName, IClientConnectionHost clientConnectionHost = null, IDiagnosticListener listener = null,
            TimeSpan? keepAlive = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            keepAlive = keepAlive ?? GetKeepAliveTimeout();
            listener = listener ?? new DiagnosticListener();
            clientConnectionHost = clientConnectionHost ?? CreateClientConnectionHost(pipeName);
            return RunServerCore(pipeName, clientConnectionHost, listener, keepAlive, cancellationToken);
        }

        internal int RunShutdown(string pipeName, bool waitForProcess = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RunShutdownAsync(pipeName, waitForProcess, timeout, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Shutting down the server is an inherently racy operation.  The server can be started or stopped by
        ///     external parties at any time.
        ///     This function will return success if at any time in the function the server is determined to no longer
        ///     be running.
        /// </summary>
        internal async Task<int> RunShutdownAsync(string pipeName, bool waitForProcess = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (WasServerRunning(pipeName) == false)
                return CommonCompiler.Succeeded;

            try
            {
                var realTimeout = timeout != null
                    ? (int) timeout.Value.TotalMilliseconds
                    : Timeout.Infinite;

                using (var client = await ConnectForShutdownAsync(pipeName, realTimeout).ConfigureAwait(false))
                {
                    var request = new Request(){IsShutDown = true};
                    RequestStreamHandler.Write(request, client); //todo make async + cancellationtoken
                    //await request.WriteAsync(client, cancellationToken).ConfigureAwait(false);

                    var response = ResponseStreamHandler.Read<Response>(client); //todo make async + cancellationtoken
                    //var response = await BuildResponse.ReadAsync(client, cancellationToken).ConfigureAwait(false);

                    if (waitForProcess)
                        try
                        {
                            var process = Process.GetProcessById(int.Parse(response.Result));
                            process.WaitForExit();
                        }
                        catch (Exception)
                        {
                            // There is an inherent race here with the server process.  If it has already shutdown
                            // by the time we try to access it then the operation has succeed.
                        }
                }

                return CommonCompiler.Succeeded;
            }
            catch (Exception)
            {
                if (WasServerRunning(pipeName) == false)
                    return CommonCompiler.Succeeded;

                return CommonCompiler.Failed;
            }
        }


        public void Stop()
        {
            RunShutdown(GetDefaultPipeName(), false);
        }
    }
}