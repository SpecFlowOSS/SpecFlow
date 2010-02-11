using System;

namespace TechTalk.SpecFlow.Parser
{
    [Serializable]
    public class ErrorDetail
    {
        public string Message { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
    }
}