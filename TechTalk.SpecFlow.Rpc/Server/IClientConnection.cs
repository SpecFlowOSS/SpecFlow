// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using BoDi;

namespace TechTalk.SpecFlow.Rpc.Server
{
    /// <summary>
    /// Abstraction over the connection to the client process.   This hides underlying connection
    /// to facilitate better testing. 
    /// </summary>
    public interface IClientConnection
    {
        /// <summary>
        /// A value which can be used to identify this connection for logging purposes only.  It has 
        /// no guarantee of uniqueness.  
        /// </summary>
        string LoggingIdentifier { get; }

        /// <summary>
        /// Server the connection and return the result.
        /// </summary>
        Task<ConnectionData> HandleConnection(bool allowCompilationRequests, CancellationToken cancellationToken, ObjectContainer container);

        /// <summary>
        /// Close the underlying client connection.
        /// </summary>
        void Close();
    }
}
