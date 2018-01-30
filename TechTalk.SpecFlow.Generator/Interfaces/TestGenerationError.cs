using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    [Serializable]
    public class TestGenerationError
    {
        /// <summary>
        /// The (zero-based) line number of the error.
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// The (zero-based) position of the error within the <see cref="Line"/>.
        /// </summary>
        public int LinePosition { get; private set; }
        /// <summary>
        /// The error message.
        /// </summary>
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
            return ex.Message + Environment.NewLine + 
                   Environment.NewLine + 
                   ex;
        }
    }
}