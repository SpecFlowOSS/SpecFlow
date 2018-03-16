using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        protected virtual void ValidateBuildRequest(BaseRequest request)
        {
        }

        /// <summary>
        /// Close the connection.  Can be called multiple times.
        /// </summary>
        public abstract void Close();

        public async Task<ConnectionData> HandleConnection(bool allowCompilationRequests = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                BaseRequest request;
                try
                {
                    Log("Begin reading request.");
                    //request = await BuildRequest.ReadAsync(_stream, cancellationToken).ConfigureAwait(false);   
                  

                    request = RequestStreamHandler.Read<Request>(_stream); //todo async + cancellationtoken
                    //ValidateBuildRequest(request);
                    Log("End reading request.");
                }
                catch (Exception e)
                {
                    LogException(e, "Error reading build request.");
                    return new ConnectionData(CompletionReason.CompilationNotStarted);
                }

                return await ExecuteRequest(request, cancellationToken).ConfigureAwait(false);
                
            }
            finally
            {
                Close();
            }
        }

        private async Task<ConnectionData> ExecuteRequest(BaseRequest baseRequest, CancellationToken cancellationToken)
        {
            //var keepAlive = CheckForNewKeepAlive(request); //todo
            var keepAlive = TimeSpan.Zero;

            if (baseRequest is ShutdownRequest)
            {
                var id = Process.GetCurrentProcess().Id;
                var response = new ShutdownResponse(id);

                ResponseStreamHandler.Write(response, _stream);
                return new ConnectionData(CompletionReason.ClientShutdownRequest);
            }


            var request = baseRequest as Request;

            if (request != null)
            {
                // Kick off both the compilation and a task to monitor the pipe for closing.
                var buildCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                var compilationTask = CreateExecutionTask(request, buildCts.Token);

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

            return new ConnectionData(CompletionReason.ClientException, keepAlive);
        }

        private Task<Response> CreateExecutionTask(Request request, CancellationToken cancellationToken)
        {
            Func<Response> func = () =>
            {
                // Do the compilation
                Log("Begin compilation");

                var response = new Response()
                {
                    Type = request.Type,
                    Method = request.Method,
                    Result = "OK"
                };

                Log("End compilation");
                return response;
            };

            var task = new Task<Response>(func, cancellationToken, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
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