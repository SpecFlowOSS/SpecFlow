using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;

namespace TechTalk.SpecFlow.Rpc.Server
{
    /// <summary>
    /// Represents a single connection from a client process. Handles the named pipe
    /// from when the client connects to it, until the request is finished or abandoned.
    /// A new task is created to actually service the connection and do the operation.
    /// </summary>
    internal abstract class ClientConnection : IClientConnection
    {
        private readonly string _loggingIdentifier;
        private readonly Stream _stream;

        public string LoggingIdentifier => _loggingIdentifier;

        public ClientConnection(string loggingIdentifier, Stream stream)
        {
            _loggingIdentifier = loggingIdentifier;
            _stream = stream;
        }

        /// <summary>
        /// Returns a Task that resolves if the client stream gets disconnected.
        /// </summary>
        protected abstract Task CreateMonitorDisconnectTask(CancellationToken cancellationToken);

        protected virtual void ValidateBuildRequest(Request request)
        {
        }

        /// <summary>
        /// Close the connection.  Can be called multiple times.
        /// </summary>
        public abstract void Close();

        public async Task<ConnectionData> HandleConnection(bool allowCompilationRequests, CancellationToken cancellationToken, ObjectContainer container)
        {
            try
            {
                Request request;
                try
                {
                    Log("Begin reading request.");

                    request = RequestStreamHandler.Read<Request>(_stream); //todo async + cancellationtoken
                    //ValidateBuildRequest(request);
                    Log("End reading request.");
                }
                catch (Exception e)
                {
                    LogException(e, "Error reading build request.");
                    return new ConnectionData(CompletionReason.CompilationNotStarted);
                }

                return await ExecuteRequest(request, container, cancellationToken).ConfigureAwait(false);

            }
            finally
            {
                Close(); 
            }
        }

        private async Task<ConnectionData> ExecuteRequest(Request request, ObjectContainer container, CancellationToken cancellationToken)
        {
            var keepAlive = CheckForNewKeepAlive(request); //todo
            

            if (request.IsShutDown)
            {
                var id = Process.GetCurrentProcess().Id;

                var response = new Response() { Result = id.ToString() };


                ResponseStreamHandler.Write(response, _stream);
                return new ConnectionData(CompletionReason.ClientShutdownRequest);
            }



            // Kick off both the compilation and a task to monitor the pipe for closing.
            var buildCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var compilationTask = CreateExecutionTask(request, container, buildCts.Token);

            var monitorTask = CreateMonitorDisconnectTask(buildCts.Token);
            await Task.WhenAny(compilationTask, monitorTask).ConfigureAwait(false);

            // Do an 'await' on the completed task, preference being compilation, to force
            // any exceptions to be realized in this method for logging.
            CompletionReason reason;
            if (compilationTask.IsCompleted)
            {
                var response = await compilationTask.ConfigureAwait(false);

                try
                {
                    Log("Begin writing response.");
                    ResponseStreamHandler.Write(response, _stream);
                    //await response.WriteAsync(_stream, cancellationToken).ConfigureAwait(false);
                    reason = CompletionReason.CompilationCompleted;
                    Log("End writing response.");
                }
                catch (Exception e)
                {
                    reason = CompletionReason.ClientDisconnect;
                    LogException(e, "");
                }
            }
            else
            {
                await monitorTask.ConfigureAwait(false);
                reason = CompletionReason.ClientDisconnect;
            }

            // Begin the tear down of the Task which didn't complete.
            buildCts.Cancel();
            return new ConnectionData(reason, keepAlive);


        }

        private TimeSpan? CheckForNewKeepAlive(Request request)
        {
            return TimeSpan.FromMinutes(10);
        }

        private Task<Response> CreateExecutionTask(Request request, ObjectContainer container, CancellationToken cancellationToken)
        {
            Func<Response> func = () =>
            {
                // Do the compilation
                Log("Begin compilation");

                var assembly = GetAssembly(request);

                var requestedType = GetRequestedType(assembly, request);

                var requestedTypeInstance = container.Resolve(requestedType);
                var requestedMethodInfo = requestedType.GetMethod(request.Method);

                var requestedArguments = JsonConvert.DeserializeObject<Dictionary<int, object>>(request.Arguments, SerializationOptions.Current);

                var result = requestedMethodInfo.Invoke(requestedTypeInstance, requestedArguments.Values.ToArray());


                var response = new Response()
                {
                    Assembly = request.Assembly,
                    Type = request.Type,
                    Method = request.Method,
                    Result = JsonConvert.SerializeObject(result, SerializationOptions.Current)
                };

                Log("End compilation");
                return response;
            };

            var task = new Task<Response>(func, cancellationToken, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
        }

        private Assembly GetAssembly(Request request)
        {
            var foundAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName == request.Assembly);

            switch (foundAssemblies.Count())
            {
                case 0:
                    throw new Exception($"No assembly {request.Assembly} found");
                case 1:
                    return foundAssemblies.Single();
                default:
                    throw new Exception($"more than one assembly {request.Assembly} were loaded");
            }
        }

        private Type GetRequestedType(Assembly assembly, Request request)
        {
            Type requestedType = null;
            try
            {
                requestedType = assembly.GetType(request.Type, true);
            }
            catch (Exception e)
            {
                Log($"Type {request.Type} not found!");
            }
            return requestedType;
        }

        private void Log(string message)
        {
            CompilerServerLogger.Log("Client {0}: {1}", _loggingIdentifier, message);
        }

        private void LogException(Exception e, string message)
        {
            CompilerServerLogger.LogException(e, string.Format("Client {0}: {1}", _loggingIdentifier, message));
        }
    }
}