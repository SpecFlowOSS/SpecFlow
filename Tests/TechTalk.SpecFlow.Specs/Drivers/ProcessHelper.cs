using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ProcessHelper
    {
        public string ConsoleOutput { get; private set; }

        public string ConsoleError { get; private set; }

        public int RunProcess(string executablePath, string argumentsFormat, params object[] arguments)
        {
            string commandArguments = string.Format(argumentsFormat, arguments);
            ProcessStartInfo psi = new ProcessStartInfo(executablePath, parameters);

            Console.WriteLine($"starting process {executablePath} {parameters}");


            Console.WriteLine("\"{0}\" {1}", executablePath, commandArguments);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = executablePath;
                process.StartInfo.Arguments = commandArguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                {
                    using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                error.AppendLine(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                        outputWaitHandle.WaitOne();
                        errorWaitHandle.WaitOne();
                    }
                }

                ConsoleOutput = output.ToString();
                Console.WriteLine(output);

                ConsoleError = error.ToString();
                Console.WriteLine(error);

                return process.ExitCode;
            }
        }
    }
}