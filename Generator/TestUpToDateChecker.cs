using System;
using System.Diagnostics;
using System.IO;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public class TestUpToDateChecker : ITestUpToDateChecker
    {
        protected readonly GeneratorInfo generatorInfo;
        private readonly ITestHeaderWriter testHeaderWriter;
        private readonly ProjectSettings projectSettings;

        public TestUpToDateChecker(ITestHeaderWriter testHeaderWriter, GeneratorInfo generatorInfo, ProjectSettings projectSettings)
        {
            this.testHeaderWriter = testHeaderWriter;
            this.generatorInfo = generatorInfo;
            this.projectSettings = projectSettings;
        }

        private bool IsUpToDateByModificationTimeAndGeneratorVersion(FeatureFileInput featureFileInput, string generatedTestFullPath)
        {
            if (generatedTestFullPath == null)
                return false;

            // check existance of the target file
            if (!File.Exists(generatedTestFullPath))
                return false;

            // check modification time of the target file
            try
            {
                var featureFileModificationTime = File.GetLastWriteTime(featureFileInput.GetFullPath(projectSettings));
                var codeFileModificationTime = File.GetLastWriteTime(generatedTestFullPath);

                if (featureFileModificationTime > codeFileModificationTime)
                    return false;

                // check tools version
                var codeFileVersion = testHeaderWriter.DetectGeneratedTestVersion(featureFileInput.GetGeneratedTestContent(generatedTestFullPath));
                if (codeFileVersion == null || codeFileVersion != generatorInfo.GeneratorVersion)
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        public bool? IsUpToDatePreliminary(FeatureFileInput featureFileInput, string generatedTestFullPath, UpToDateCheckingMethod upToDateCheckingMethod)
        {
            bool byUpdateTimeCheckResult = IsUpToDateByModificationTimeAndGeneratorVersion(featureFileInput, generatedTestFullPath);
            if (upToDateCheckingMethod == UpToDateCheckingMethod.ModificationTimeAndGeneratorVersion)
                return byUpdateTimeCheckResult;

            if (byUpdateTimeCheckResult == false)
                return false;

            return null;
        }

        public bool IsUpToDate(FeatureFileInput featureFileInput, string generatedTestFullPath, string generatedTestContent, UpToDateCheckingMethod upToDateCheckingMethod)
        {
            string existingFileContent = featureFileInput.GetGeneratedTestContent(generatedTestFullPath);
            return existingFileContent != null && existingFileContent.Equals(generatedTestContent, StringComparison.InvariantCulture);
        }
    }
}