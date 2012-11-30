using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TechTalk.SpecFlow.IdeIntegration.Tracing
{
    public class FileTracer : IIdeTracer
    {
        private readonly object fileLock = new object();
        private readonly string logFilePath;

        public FileTracer()
        {
            logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.Format(@"SpecFlow\Logs\vslog_{0}.log", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
            string folder = Path.GetDirectoryName(logFilePath);
            Debug.Assert(folder != null);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public void Trace(string message, string category)
        {
            DateTime now = DateTime.Now;
            var fullMessage = string.Format("[{2}] {0}: {1}{3}", category, message, now.TimeOfDay, Environment.NewLine);

            lock (fileLock)
            {
                File.AppendAllText(logFilePath, fullMessage, Encoding.UTF8);
            }
        }

        public bool IsEnabled(string category)
        {
            return true;
        }
    }
}