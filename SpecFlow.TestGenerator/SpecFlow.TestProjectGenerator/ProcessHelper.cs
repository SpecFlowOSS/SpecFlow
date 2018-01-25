using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace SpecFlow.TestProjectGenerator
{
    public class ProcessHelper
    {
        private static TimeSpan _timeout = TimeSpan.FromMinutes(10);
        private static int _timeOutInMilliseconds = Convert.ToInt32(_timeout.TotalMilliseconds);

        public string ConsoleOutput { get; private set; }

        public int RunProcess(string executablePath, string argumentsFormat, params object[] arguments)
        {
            var parameters = string.Format(argumentsFormat, arguments);

            Console.WriteLine($"Starting external program: \"{executablePath}\" {parameters}");
            ProcessStartInfo psi = new ProcessStartInfo(executablePath, parameters);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;


            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true,

            };

            StringBuilder output = new StringBuilder();
            

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
                            output.AppendLine(e.Data);
                        }
                    };


                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(_timeOutInMilliseconds) &&
                        outputWaitHandle.WaitOne(_timeOutInMilliseconds) &&
                        errorWaitHandle.WaitOne(_timeOutInMilliseconds))
                    {
                        ConsoleOutput = output.ToString();
                    }
                    else
                    {
                        throw new TimeoutException($"Process {psi.FileName} {psi.Arguments} took longer than {_timeout.TotalMinutes} min to complete");
                    }

                }
            }
            Console.WriteLine(ConsoleOutput);


            return process.ExitCode;
        }
    }
}