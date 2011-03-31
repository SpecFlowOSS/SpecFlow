using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class GenerationSettings
    {
        public bool CheckUpToDate { get; set; }
        public bool WriteResultToFile { get; set; }

        public GenerationSettings()
        {
            CheckUpToDate = false;
            WriteResultToFile = false;
        }
    }
}