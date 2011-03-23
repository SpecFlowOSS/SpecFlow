using System;

namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public class TestGenerationError
    {
        /// <summary>
        /// Zero-based indexing!
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// Zero-based indexing!
        /// </summary>
        public int LinePosition { get; private set; }

        public string Message { get; private set; }

        public TestGenerationError(int line, int linePosition, string message)
        {
            Line = line;
            LinePosition = linePosition;
            Message = message;
        }
    }
}