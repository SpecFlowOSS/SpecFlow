
using Microsoft.Build.Framework;

namespace TechTalk.SpecFlow.MSBuildTasks
{
    public class GenerateFeatureFileCodeBehindTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string ProjectPath { get; set; }
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }
        public override bool Execute()
        {

            foreach (var featureFile in FeatureFiles)
            {
                Log.LogMessage(MessageImportance.High, featureFile.ItemSpec);
            }

            Log.LogMessage(MessageImportance.High, "Aloha");
            return true;
        }
    }

}