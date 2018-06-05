using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCOREAPP
using System.Runtime.Loader;
#endif
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Task
    {
        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        public override bool Execute()
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();

                Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");

                var generatedFiles = new List<ITaskItem>();

                foreach (string s in GenerateFilesForProject())
                {
                    generatedFiles.Add(new TaskItem() { ItemSpec = s });
                }

                GeneratedFiles = generatedFiles.ToArray();

                return true;
            }
            catch (Exception e)
            {
                Log?.LogWithNameTag(Log.LogError, e.Demystify().ToString());
                return false;
            }
        }

#if NETCOREAPP
        private IEnumerable<string> GenerateFilesForProject()
        {
            string taskAssemblyPath = new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath;
            var ctxt = new CustomAssemblyLoader();
            Assembly inContextAssembly = ctxt.LoadFromAssemblyPath(taskAssemblyPath);
            Type innerTaskType = inContextAssembly.GetType(typeof(FeatureFileCodeBehindGenerator).FullName);
            object innerTask = Activator.CreateInstance(innerTaskType);

            var executeInnerMethod = innerTaskType.GetMethod("GenerateFilesForProject", BindingFlags.Instance | BindingFlags.Public);
            var result = (IEnumerable<string>)executeInnerMethod.Invoke(innerTask, new object[]{ProjectPath, RootNamespace, FeatureFiles.Select(i => i.ItemSpec).ToList(), ProjectFolder, OutputPath });
            return result;
        }

        private class CustomAssemblyLoader : AssemblyLoadContext
        {
            protected virtual string ManagedDllDirectory => Path.GetDirectoryName(new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath);

            protected override Assembly Load(AssemblyName assemblyName)
            {
                string assemblyPath = Path.Combine(this.ManagedDllDirectory, assemblyName.Name) + ".dll";
                if (File.Exists(assemblyPath))
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }

                return Default.LoadFromAssemblyName(assemblyName);
            }
        }


#else
        private IEnumerable<string> GenerateFilesForProject()
        {
            var generator = new FeatureFileCodeBehindGenerator();
            return generator.GenerateFilesForProject(ProjectPath, RootNamespace, FeatureFiles.Select(i => i.ItemSpec).ToList(), ProjectFolder, OutputPath);
        }
#endif
    }


}