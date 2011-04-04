using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public static class FeatureFileInputExtensions
    {
        public static TextReader GetFeatureFileContentReader(this FeatureFileInput featureFileInput, ProjectSettings projectSettings)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");

            if (featureFileInput.FeatureFileContent != null)
                return new StringReader(featureFileInput.FeatureFileContent);

            Debug.Assert(projectSettings != null);

            return new StreamReader(Path.Combine(projectSettings.ProjectFolder, featureFileInput.ProjectRelativePath));
        }

        public static string GetFullPath(this FeatureFileInput featureFileInput, ProjectSettings projectSettings)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");

            if (projectSettings == null)
                return featureFileInput.ProjectRelativePath;

            return Path.GetFullPath(Path.Combine(projectSettings.ProjectFolder, featureFileInput.ProjectRelativePath));
        }

        public static string GetGeneratedTestFullPath(this FeatureFileInput featureFileInput, ProjectSettings projectSettings)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");

            if (featureFileInput.GeneratedTestProjectRelativePath == null)
                return null;

            if (projectSettings == null)
                return featureFileInput.GeneratedTestProjectRelativePath;

            return Path.GetFullPath(Path.Combine(projectSettings.ProjectFolder, featureFileInput.GeneratedTestProjectRelativePath));
        }

        public static string GetGeneratedTestContent(this FeatureFileInput featureFileInput, string generatedTestFullPath)
        {
            var generatedTestFileContent = featureFileInput.GeneratedTestFileContent;
            if (generatedTestFileContent != null)
                return generatedTestFileContent;

            if (generatedTestFullPath == null)
                return null;

            try
            {
                if (!File.Exists(generatedTestFullPath))
                    return null;

                return File.ReadAllText(generatedTestFullPath);
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception, "FeatureFileInputExtensions.GetGeneratedTestContent");
                return null;
            }
        }
    }
}
