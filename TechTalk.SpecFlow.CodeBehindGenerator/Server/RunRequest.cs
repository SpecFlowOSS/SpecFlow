// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    internal struct RunRequest
    {
        public string Language { get; }
        public string CurrentDirectory { get; }
        public string TempDirectory { get; }
        public string LibDirectory { get; }
        public string[] Arguments { get; }

        //public RunRequest(string language, string currentDirectory, string tempDirectory, string libDirectory, string[] arguments)
        //{
        //    Language = language;
        //    CurrentDirectory = currentDirectory;
        //    TempDirectory = tempDirectory;
        //    LibDirectory = libDirectory;
        //    Arguments = arguments;
        //}
    }
}
