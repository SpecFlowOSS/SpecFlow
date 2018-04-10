// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// This file has been modified by TechTalk.

using System;

namespace TechTalk.SpecFlow.Rpc.Server
{
    public struct ConnectionData
    {
        public readonly CompletionReason CompletionReason;
        public readonly TimeSpan? KeepAlive;

        public ConnectionData(CompletionReason completionReason, TimeSpan? keepAlive = null)
        {
            CompletionReason = completionReason;
            KeepAlive = keepAlive;
        }
    }
}
