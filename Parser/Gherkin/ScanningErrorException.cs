using System;
using System.Diagnostics;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public class ScanningErrorException : Exception
    {
        private readonly GherkinBufferPosition position;
        private int? line;
        private int? linePosition;

        public ScanningErrorException(string message) : base(message)
        {
        }

        public ScanningErrorException(string message, GherkinBufferPosition position) : base(message)
        {
            this.position = position;
        }

        public ScanningErrorException(string message, int line, int linePosition) : base(message)
        {
            this.line = line;
            this.linePosition = linePosition;
        }

        public GherkinBufferPosition GetPosition(GherkinBuffer gherkinBuffer)
        {
            if (position != null)
            {
                Debug.Assert(position.Buffer == gherkinBuffer);
                return position;
            }
            if (line != null)
            {
                return linePosition == null ? gherkinBuffer.GetLineStartPosition(line.Value)
                    : new GherkinBufferPosition(gherkinBuffer, line.Value, linePosition.Value);
            }
            return null;
        }
    }
}