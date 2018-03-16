// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    internal sealed class EmptyDiagnosticListener : IDiagnosticListener
    {
        public void UpdateKeepAlive(TimeSpan timeSpan)
        {
        }

        public void ConnectionListening()
        {
        }

        public void ConnectionReceived()
        {
        }

        public void ConnectionCompleted(int count)
        {
        }

        public void ConnectionRudelyEnded()
        {
        }

        public void KeepAliveReached()
        {
        }
    }
}
