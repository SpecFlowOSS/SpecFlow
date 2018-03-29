using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class CommandLineOptions
    {
        [Option]
        public int Port { get; set; }

        [Option]
        public bool Debug { get; set; }
    }
}
