using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Specs.MSBuild.Support
{
    public class TestAssemblyInfo
    {
        private static readonly string AnalyzerPath;

        private static readonly Regex ScenarioPattern = new Regex(@"^\{(?<feature>.*)\}\:\{(?<scenario>.*)\}$");

        static TestAssemblyInfo()
        {
            const string configuration =
#if DEBUG
                "Debug";
#else
                "Release";
#endif

            const string target =
#if NETCOREAPP
                "netcoreapp2.2";
#else
                "net48";
#endif

            AnalyzerPath = $"../../../../TechTalk.SpecFlow.Specs.MSBuild.Analyzer/bin/{configuration}/{target}/TechTalk.SpecFlow.Specs.MSBuild.Analyzer.dll";
        }

        public TestAssemblyInfo(IReadOnlyCollection<TestFeatureInfo> features)
        {
            Features = features ?? throw new ArgumentNullException(nameof(features));
        }

        public IReadOnlyCollection<TestFeatureInfo> Features { get; }

        public static TestAssemblyInfo ReadTestAssembly(string path)
        {
            var start = new ProcessStartInfo
            {
                FileName = "dotnet",
                ArgumentList = { AnalyzerPath, path },
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(start))
            {
                var features = new HashSet<string>();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();

                    var match = ScenarioPattern.Match(line);
                    if (match.Success)
                    {
                        features.Add(match.Groups["feature"].Value);
                    }
                }

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException("Analyzer failed to process test assembly.");
                }

                return new TestAssemblyInfo(features.Select(f => new TestFeatureInfo { Title = f }).ToList());
            }
        }
    }
}
