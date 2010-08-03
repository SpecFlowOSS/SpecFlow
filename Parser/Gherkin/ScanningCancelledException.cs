using System;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public class ScanningCancelledException : Exception
    {
        public ScanningCancelledException()
        {
        }

        public ScanningCancelledException(string message) : base(message)
        {
        }

        public ScanningCancelledException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}