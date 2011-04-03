using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// <summary>
    /// Settings for test generation
    /// </summary>
    [Serializable]
    public class GenerationSettings
    {
        /// <summary>
        /// Specifies whether the generator should check the existing generation results. 
        /// Optional, disabled by default.
        /// </summary>
        public bool CheckUpToDate { get; set; }
        /// <summary>
        /// Specifies whether the generation result should be written out to a file. Optional, 
        /// disabled by default.
        /// </summary>
        public bool WriteResultToFile { get; set; }

        public GenerationSettings()
        {
            CheckUpToDate = false;
            WriteResultToFile = false;
        }
    }
}