// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace TechTalk.SpecFlow.Rpc.Server
{
    internal sealed class DiagnosticListener : IDiagnosticListener
    {
        public void UpdateKeepAlive(TimeSpan timeSpan)
        {
            Debug.WriteLine(nameof(UpdateKeepAlive));

        }

        public void ConnectionListening()
        {
            Debug.WriteLine(nameof(ConnectionListening));
        }

        public void ConnectionReceived()
        {
            Debug.WriteLine(nameof(ConnectionReceived));

        }

        public void ConnectionCompleted(int count)
        {
            Debug.WriteLine(nameof(ConnectionCompleted));

        }

        public void ConnectionRudelyEnded()
        {
            Debug.WriteLine(nameof(ConnectionRudelyEnded));

        }

        public void KeepAliveReached()
        {
            Debug.WriteLine(nameof(KeepAliveReached));

        }
    }
}
