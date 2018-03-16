using System;

namespace TechTalk.SpecFlow.Rpc.Server
{
    public interface IDiagnosticListener
    {
        /// <summary>
        /// Called when the server updates the keep alive value.
        /// </summary>
        void UpdateKeepAlive(TimeSpan timeSpan);

        /// <summary>
        /// Called each time the server listens for new connections.
        /// </summary>
        void ConnectionListening();

        /// <summary>
        /// Called when a connection to the server occurs.
        /// </summary>
        void ConnectionReceived();

        /// <summary>
        /// Called when one or more connections have completed processing.  The number of connections
        /// processed is provided in <paramref name="count"/>.
        /// </summary>
        void ConnectionCompleted(int count);

        /// <summary>
        /// Called when a bad client connection was detected and the server will be shutting down as a 
        /// result.
        /// </summary>
        void ConnectionRudelyEnded();

        /// <summary>
        /// Called when the server is shutting down because the keep alive timeout was reached.
        /// </summary>
        void KeepAliveReached();
    }
}