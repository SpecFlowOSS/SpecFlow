using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Tools.MsBuild
{
    public class GenerateAll : GeneratorTaskBase
    {
        public bool VerboseOutput { get; set; }
        public bool ForceGeneration { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        private readonly List<ITaskItem> generatedFiles = new List<ITaskItem>();
        [Output]
        public ITaskItem[] GeneratedFiles
        {
            get { return generatedFiles.ToArray(); }
        }

        protected override void DoExecute()
        {
            ITraceListener traceListener = VerboseOutput ? (ITraceListener)new TextWriterTraceListener(GetMessageWriter(MessageImportance.High), "SpecFlow: ") : new NullListener();

            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(ProjectPath);

            BatchGenerator batchGenerator = new MsBuildBatchGenerator(traceListener, new TestGeneratorFactory(), this);
            batchGenerator.OnError +=
                delegate(FeatureFileInput featureFileInput, TestGeneratorResult result)
                    {
                        foreach (var testGenerationError in result.Errors)
                        {
                            RecordError(testGenerationError.Message, 
                                featureFileInput.GetFullPath(specFlowProject.ProjectSettings), testGenerationError.Line, testGenerationError.LinePosition);
                        }
                    };
            batchGenerator.OnSuccess +=
                (featureFileInput, result) => generatedFiles.Add(
                    new TaskItem(featureFileInput.GetGeneratedTestFullPath(specFlowProject.ProjectSettings)));

            batchGenerator.ProcessProject(specFlowProject, ForceGeneration);
        }
    }
}