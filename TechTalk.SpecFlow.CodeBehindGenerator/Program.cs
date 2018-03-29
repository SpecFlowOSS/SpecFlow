using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoDi;
using CommandLine;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<CommandLineOptions>(HandleParseError);


        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {

        }

        private static int RunOptionsAndReturnExitCode(CommandLineOptions opts)
        {
            if (opts.Debug)
            {
                Debugger.Launch();
            }


            var container = new ObjectContainer();


            var buildServerController = new BuildServerController(container);
            container.RegisterInstanceAs(buildServerController);
            container.RegisterTypeAs<FeatureCodeBehindGenerator, IFeatureCodeBehindGenerator>();

            return buildServerController.Run(opts.Port);
        }
    }
}
