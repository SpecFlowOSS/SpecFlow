using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoDi;
using CommandLine;
using Serilog;
using Serilog.Core;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class Program
    {
        private static Logger _log;

        static void Main(string[] args)
        {
            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();


            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<CommandLineOptions>(HandleParseError);


        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {

        }

        private static int RunOptionsAndReturnExitCode(CommandLineOptions opts)
        {
            try
            {
                _log.Information("TechTalk.SpecFlow.CodeBehindGenerator started");
                if (opts.Debug)
                {
                    Debugger.Launch();
                }

                var container = new ObjectContainer();

                var buildServerController = new BuildServerController(container, _log);
                container.RegisterInstanceAs(buildServerController);
                container.RegisterInstanceAs(_log);
                container.RegisterTypeAs<FeatureCodeBehindGenerator, IFeatureCodeBehindGenerator>();

                _log.Information("Starting on port {0}", opts.Port);

                return buildServerController.Run(opts.Port);
            }
            catch (Exception e)
            {
                _log.Error(e, "Error starting TechTalk.SpecFlow.CodeBehindGenerator");
                return 999;
            }
        }
    }
}
