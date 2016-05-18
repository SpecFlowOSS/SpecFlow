using System;
using TechTalk.SpecFlow.Generator;
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

        protected override FeatureFileInput CreateFeatureFileInput(FeatureFileInput featureFile, ITestGenerator generator, SpecFlowProject specFlowProject)
        {
            var featureFileInput = base.CreateFeatureFileInput(featureFile, generator, specFlowProject);

            outputFile = task.PrepareOutputFile(generator.GetTestFullPath(featureFileInput));
            featureFileInput.GeneratedTestProjectRelativePath =
                FileSystemHelper.GetRelativePath(outputFile.FilePathForWriting, specFlowProject.ProjectSettings.ProjectFolder);
            return featureFileInput;
        }

        protected override GenerationSettings GetGenerationSettings(bool forceGenerate)
        {
            var settings = base.GetGenerationSettings(forceGenerate);
            settings.UpToDateCheckingMethod = UpToDateCheckingMethod.FileContent;
            return settings;
        }

        protected override TestGeneratorResult GenerateTestFile(ITestGenerator generator, FeatureFileInput featureFileInput, GenerationSettings generationSettings)
        {
            try
            {
                var result = base.GenerateTestFile(generator, featureFileInput, generationSettings);

                if (result.IsUpToDate)
                    outputFile.Skip();
                else
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
}