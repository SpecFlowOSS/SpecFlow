using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{  
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    /// 
    /// 
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
        /// Specifies the way how the up-to-date checking should be performed.
        /// </summary>
        public UpToDateCheckingMethod UpToDateCheckingMethod { get; set; }
        
        /// <summary>
        /// Specifies whether the generation result should be written out to a file. Optional, 
        /// disabled by default.
        /// </summary>
        public bool WriteResultToFile { get; set; }

        public GenerationSettings()
        {
            CheckUpToDate = false;
            UpToDateCheckingMethod = UpToDateCheckingMethod.ModificationTimeAndGeneratorVersion;

            WriteResultToFile = false;
        }
    }

    public enum UpToDateCheckingMethod
    {
        ModificationTimeAndGeneratorVersion,
        FileContent
    }
}