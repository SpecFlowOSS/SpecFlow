using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpecFlow.Tools.MsBuild.Generation.FrameworkDependent
{
    public class GenerateFeatureFileCodeBehind
    {
        public IEnumerable<string> GenerateFilesForProject(List<string> generatorPlugins, string projectPath, string projectFolder, string outputPath, string rootNamespace, List<string> featureFiles)
        {
            string taskAssemblyPath = new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath;
            var ctxt = new CustomAssemblyLoader();
            Assembly inContextAssembly = ctxt.LoadFromAssemblyPath(taskAssemblyPath);
            Type innerTaskType = inContextAssembly.GetType(typeof(FeatureFileCodeBehindGenerator).FullName);
            object innerTask = Activator.CreateInstance(innerTaskType);

            var executeInnerMethod = innerTaskType.GetMethod("GenerateFilesForProject", BindingFlags.Instance | BindingFlags.Public);

            var parameters = new object[] { projectPath, rootNamespace, featureFiles, generatorPlugins, projectFolder, outputPath };
            var result = (IEnumerable<string>)executeInnerMethod.Invoke(innerTask, parameters);

            return result;
        }
    }
}