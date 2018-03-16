
using System;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.CodeBehindGenerator.Client;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared;

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


                using (var client = new Client<IFeatureCodeBehindGenerator>())
                {
                    client.Execute(c => c.InitializeProject(ProjectPath));

                    foreach (var featureFile in FeatureFiles)
                    {
                        client.Execute(c => c.GenerateCodeBehindFile(featureFile.ItemSpec));
                    }
                }

                //using (var client = new RawClient(4658))
                //{
                
                //}
                
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