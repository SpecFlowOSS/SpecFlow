using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
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

        public TestGenerationError(Exception exception) : 
            this(0, 0, "Generation error: " + GetMessage(exception))
        {
        }

        private static string GetMessage(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;

            return ex.Message + " -> " + GetMessage(ex.InnerException);
        }

    }
}