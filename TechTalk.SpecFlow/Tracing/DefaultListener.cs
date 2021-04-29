using System;
using System.Linq;

namespace TechTalk.SpecFlow.Tracing
{
    public class DefaultListener : ITraceListener
    {
        public void WriteTestOutput(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteToolOutput(string message)
        {
            Console.WriteLine("-> " + message);
        }

        public void AddAttachment(string filePath)
        {
            WriteTestOutput($"Attachment added: {filePath}");
        }
    }
}