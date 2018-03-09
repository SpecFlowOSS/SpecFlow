
using System;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.CodeBehindGenerator.Client;

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
            try
            {
                StartOutOfProcGenerator();

                using (var client = new Client(4658))
                {
                
                }
                
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }

        private void StartOutOfProcGenerator()
        {
            //todo
        }
    }

}