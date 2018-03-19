using System;
using BoDi;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new ObjectContainer();


            new BuildServerController(container).Run(4759);
        }
    }
}
