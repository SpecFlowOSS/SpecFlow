using System;
using TechTalk.SpecFlow.CodeBehindGenerator.Server;

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
