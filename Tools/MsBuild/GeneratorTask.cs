using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Tools.MsBuild
{
    internal class MsBuildBatchGenerator : BatchGenerator
    {
        private readonly GeneratorTaskBase task;

        public MsBuildBatchGenerator(TextWriter traceWriter, bool verboseOutput, GeneratorTaskBase task) : base(traceWriter, verboseOutput)
        {
            this.task = task;
        }

        private GeneratorTaskBase.OutputFile outputFile = null;

        protected override StreamWriter GetWriter(string codeFileFullPath)
        {
            outputFile = task.PrepareOutputFile(codeFileFullPath);

            return base.GetWriter(outputFile.FilePathForWriting);
        }

        protected override void GenerateFile(SpecFlowGenerator generator, SpecFlowFeatureFile featureFile, string codeFileFullPath)
        {
            try
            {
                base.GenerateFile(generator, featureFile, codeFileFullPath);
                outputFile.Done(task.Errors);
            }
            catch(Exception ex)
            {
                task.RecordException(ex);
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

            BatchGenerator batchGenerator = new MsBuildBatchGenerator(
                GetMessageWriter(MessageImportance.High), VerboseOutput, this);
            batchGenerator.ProcessProject(specFlowProject, ForceGeneration);
        }
    }
}