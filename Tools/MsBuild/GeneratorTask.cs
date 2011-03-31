using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Tools.MsBuild
{
    internal class MsBuildBatchGenerator : BatchGenerator
    {
        private readonly GeneratorTaskBase task;

        public MsBuildBatchGenerator(ITraceListener traceListener, ITestGeneratorFactory testGeneratorFactory, GeneratorTaskBase task) : base(traceListener, testGeneratorFactory)
        {
            this.task = task;
        }

        private GeneratorTaskBase.OutputFile outputFile = null;

        protected override FeatureFileInput CreateFeatureFileInput(SpecFlowFeatureFile featureFile, ITestGenerator generator, SpecFlowProject specFlowProject)
        {
            var featureFileInput = base.CreateFeatureFileInput(featureFile, generator, specFlowProject);

            outputFile = task.PrepareOutputFile(generator.GetTestFullPath(featureFileInput));
            featureFileInput.GeneratedTestProjectRelativePath =
                FileSystemHelper.GetRelativePath(outputFile.FilePathForWriting, specFlowProject.ProjectSettings.ProjectFolder);
            return featureFileInput;
        }

        protected override TestGeneratorResult GenerateTestFile(ITestGenerator generator, FeatureFileInput featureFileInput, GenerationSettings generationSettings)
        {
            try
            {
                var result = base.GenerateTestFile(generator, featureFileInput, generationSettings);
                outputFile.Done(task.Errors);
                return result;
            }
            catch (Exception ex)
            {
                task.RecordException(ex);
                return new TestGeneratorResult(new TestGenerationError(ex));
            }
            finally
            {
                outputFile = null;
            }
        }
    }

    public class GenerateAll : GeneratorTaskBase
    {
        public bool VerboseOutput { get; set; }
        public bool ForceGeneration { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        protected override void DoExecute()
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(ProjectPath);

            ITraceListener traceListener = VerboseOutput ? (ITraceListener)new TextWriterTraceListener(GetMessageWriter(MessageImportance.High)) : new NullListener();
            BatchGenerator batchGenerator = new MsBuildBatchGenerator(traceListener, new TestGeneratorFactory(), this);
            batchGenerator.ProcessProject(specFlowProject, ForceGeneration);
        }
    }
}