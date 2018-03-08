// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    internal sealed class CoreClrCompilerServerHost : CompilerServerHost
    {
       
        internal CoreClrCompilerServerHost(string clientDirectory)
            : base(clientDirectory: clientDirectory, sdkDirectory: null)
        {
            
        }

    }
}
