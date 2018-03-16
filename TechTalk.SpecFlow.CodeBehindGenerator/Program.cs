using System;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            new BuildServerController().Run(4759);
        }
    }
}
