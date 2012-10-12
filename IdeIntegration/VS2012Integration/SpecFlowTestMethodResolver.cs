using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestWindow.Controller;
using Microsoft.VisualStudio.TestWindow.Data;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.VsAdapters;

namespace TechTalk.SpecFlow.Vs2012Integration
{
    [Export(typeof(ITestMethodResolver))]
    public class SpecFlowTestMethodResolver : ITestMethodResolver
    {
        public Uri ExecutorUri
        {
            get { return new Uri("executor://SpecFlow/v1"); }
        }

        private readonly VsTestMethodResolver defaultResolver = null;

        [ImportingConstructor]
        public SpecFlowTestMethodResolver(Lazy<ITestContainerProvider> testContainerProvider, SafeDispatcher safeDispatcher)
        {
            defaultResolver = new VsTestMethodResolver(testContainerProvider, safeDispatcher);
        }

        public string GetCurrentTest(string filePath, int line, int lineCharOffset)
        {
            if (filePath.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase))
            {
                var codeBehindPath = filePath + ".cs";
                int codeBehindLine = GetCodeBehindLine(codeBehindPath, line);
                if (codeBehindLine < 0)
                {
                    return null; // run the entire file
                }

                var currentTest = defaultResolver.GetCurrentTest(codeBehindPath, codeBehindLine, 0);
                return currentTest;
            }

            return null;
        }

        private int GetCodeBehindLine(string filePath, int line)
        {
            using (var reader = new StreamReader(filePath))
            {
                return GetCodeBehindLine(reader, line);
            }
        }

        private static readonly Regex linePragmaRe = new Regex(@"^((#line\s+(?<lineno>\d+))|(#ExternalSource\("".*"",(?<lineno>\d+)\)))\s*$");

        protected int GetCodeBehindLine(TextReader reader, int sourceLine)
        {
            int sourceLine1Based = sourceLine + 1;
            int startLine = 0;
            string lineText;
            while ((lineText = reader.ReadLine()) != null)
            {
                //#line 8
                //#ExternalSource("SpecFlowFeature2.feature",8)
                var match = linePragmaRe.Match(lineText);
                if (match.Success)
                {
                    int linePragmaValue;
                    if (int.TryParse(match.Groups["lineno"].Value, out linePragmaValue))
                    {
                        if (linePragmaValue >= sourceLine1Based)
                        {
                            return startLine + 1; // the next line after the pragma
                        }
                    }
                }
                startLine++;
            }

            return -1;
        }
    }
}
