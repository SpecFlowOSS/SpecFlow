using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ProcessHelper
    {
        public string ConsoleOutput { get; private set; }

        public int RunProcess(string executablePath, string argumentsFormat, params object[] arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo(executablePath, string.Format(argumentsFormat, arguments));
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            var p = Process.Start(psi);

            ConsoleOutput = p.StandardOutput.ReadToEnd();
            Console.WriteLine(ConsoleOutput);

            p.WaitForExit();

            return p.ExitCode;
        }
    }
}