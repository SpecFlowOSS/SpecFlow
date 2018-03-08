﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{

    internal class CommonCompiler
    {
        public static int Succeeded { get; set; }
        public static int Failed { get; set; }
    }

    /// <summary>
    /// Base type for the build server code.  Contains the basic logic for running the actual server, startup 
    /// and shutdown.
    /// </summary>
    public class BuildServerController
    {
        public int Run(int port)
        {
            DefaultPort = port;
            bool shutdown = false;


            var pipeName = GetDefaultPipeName();
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => { cancellationTokenSource.Cancel(); };

            return shutdown
                ? RunShutdown(pipeName, cancellationToken: cancellationTokenSource.Token)
                : RunServer(pipeName, cancellationToken: cancellationTokenSource.Token);
        }

        // As a proof of concept implementation the portable code is using TCP as a communication
        // mechanism.  This will eventually switch to domain sockets or named pipes
        // https://github.com/dotnet/roslyn/issues/9696

        internal int DefaultPort = 4242;
        internal const string DefaultAddress = "127.0.0.1";

        protected async Task<Stream> ConnectForShutdownAsync(string pipeName, int timeout)
        {
            var port = int.Parse(pipeName);
            var ipAddress = IPAddress.Parse(DefaultAddress);
            var client = new TcpClient();
            await client.ConnectAsync(ipAddress, port).ConfigureAwait(false);
            return client.GetStream();
        }

        protected IClientConnectionHost CreateClientConnectionHost(string pipeName)
        {
            var port = int.Parse(pipeName);
            var ipAddress = IPAddress.Parse(DefaultAddress);
            var endPoint = new IPEndPoint(ipAddress, port: port);
            var clientDirectory = AppContext.BaseDirectory;
            var compilerHost = new CoreClrCompilerServerHost(clientDirectory);
            var connectionHost = new TcpClientConnectionHost(compilerHost, endPoint);
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
        /// Was a server running with the specified session key during the execution of this call?
        /// </summary>
        protected virtual bool? WasServerRunning(string pipeName)
        {
            return null;
        }

        protected virtual int RunServerCore(string pipeName, IClientConnectionHost connectionHost, IDiagnosticListener listener, TimeSpan? keepAlive, CancellationToken cancellationToken)
        {
            CompilerServerLogger.Log("Keep alive timeout is: {0} milliseconds.", keepAlive?.TotalMilliseconds ?? 0);
            FatalError.Handler = FailFast.OnFatalException;

            var dispatcher = new ServerDispatcher(connectionHost, listener);
            dispatcher.ListenAndDispatchConnections(keepAlive, cancellationToken);
            return CommonCompiler.Succeeded;
        }

        internal int RunServer(string pipeName, IClientConnectionHost clientConnectionHost = null, IDiagnosticListener listener = null, TimeSpan? keepAlive = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            keepAlive = keepAlive ?? GetKeepAliveTimeout();
            listener = listener ?? new EmptyDiagnosticListener();
            clientConnectionHost = clientConnectionHost ?? CreateClientConnectionHost(pipeName);
            return RunServerCore(pipeName, clientConnectionHost, listener, keepAlive, cancellationToken);
        }

        internal int RunShutdown(string pipeName, bool waitForProcess = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RunShutdownAsync(pipeName, waitForProcess, timeout, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Shutting down the server is an inherently racy operation.  The server can be started or stopped by
        /// external parties at any time.
        /// 
        /// This function will return success if at any time in the function the server is determined to no longer
        /// be running.
        /// </summary>
        internal async Task<int> RunShutdownAsync(string pipeName, bool waitForProcess = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (WasServerRunning(pipeName) == false)
            {
                // The server holds the mutex whenever it is running, if it's not open then the 
                // server simply isn't running.
                return CommonCompiler.Succeeded;
            }

            try
            {
                var realTimeout = timeout != null
                    ? (int)timeout.Value.TotalMilliseconds
                    : Timeout.Infinite;
                using (var client = await ConnectForShutdownAsync(pipeName, realTimeout).ConfigureAwait(false))
                {
                    var request = BuildRequest.CreateShutdown();
                    await request.WriteAsync(client, cancellationToken).ConfigureAwait(false);
                    var response = await BuildResponse.ReadAsync(client, cancellationToken).ConfigureAwait(false);
                    var shutdownResponse = (ShutdownBuildResponse)response;

                    if (waitForProcess)
                    {
                        try
                        {
                            var process = Process.GetProcessById(shutdownResponse.ServerProcessId);
                            process.WaitForExit();
                        }
                        catch (Exception)
                        {
                            // There is an inherent race here with the server process.  If it has already shutdown
                            // by the time we try to access it then the operation has succeed.
                        }
                    }
                }

                return CommonCompiler.Succeeded;
            }
            catch (Exception)
            {
                if (WasServerRunning(pipeName) == false)
                {
                    // If the server was in the process of shutting down when we connected then it's reasonable
                    // for an exception to happen.  If the mutex has shutdown at this point then the server 
                    // is shut down.
                    return CommonCompiler.Succeeded;
                }

                return CommonCompiler.Failed;
            }
        }

        internal static bool ParseCommandLine(string[] args, out string pipeName, out bool shutdown)
        {
            pipeName = null;
            shutdown = false;

            foreach (var arg in args)
            {
                const string pipeArgPrefix = "-pipename:";
                if (arg.StartsWith(pipeArgPrefix, StringComparison.Ordinal))
                {
                    pipeName = arg.Substring(pipeArgPrefix.Length);
                }
                else if (arg == "-shutdown")
                {
                    shutdown = true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void Stop()
        {
            RunShutdown(GetDefaultPipeName(), false);
        }
    }

    //internal class CompilerServerLogger
    //{
    //    public static void Log(string keepAliveTimeoutIsMilliseconds, double d)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static void LogException(Exception exception, string format)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static void Log(string keepAliveTimeoutIsMilliseconds, string loggingIdentifier, string message)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
