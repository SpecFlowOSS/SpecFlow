namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    public enum CompletionReason
    {
        /// <summary>
        /// There was an error creating the <see cref="BuildRequest"/> object and a compilation was never
        /// created.
        /// </summary>
        CompilationNotStarted,

        /// <summary>
        /// The compilation completed and results were provided to the client.
        /// </summary>
        CompilationCompleted,

        /// <summary>
        /// The compilation process was initiated and the client disconnected before
        /// the results could be provided to them.
        /// </summary>
        ClientDisconnect,

        /// <summary>
        /// There was an unhandled exception processing the result.
        /// </summary>
        ClientException,

        /// <summary>
        /// There was a request from the client to shutdown the server.
        /// </summary>
        ClientShutdownRequest,
    }
}