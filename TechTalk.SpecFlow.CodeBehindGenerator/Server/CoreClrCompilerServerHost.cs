// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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
