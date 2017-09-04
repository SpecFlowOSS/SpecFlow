using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SpecFlow.xUnitAdapter.SpecFlowPlugin.TestArtifacts
{
    [Serializable]
    public class SpecFlowProjectAssemblyInfo : LongLivedMarshalByRefObject, IAssemblyInfo, IXunitSerializable
    {
        public string FeatureFilesFolder => Path.GetFullPath(Path.GetDirectoryName(originalAssemblyInfo.AssemblyPath));

        private IAssemblyInfo originalAssemblyInfo;

        public SpecFlowProjectAssemblyInfo()
        {
            
        }

        public SpecFlowProjectAssemblyInfo(IAssemblyInfo originalAssemblyInfo)
        {
            this.originalAssemblyInfo = originalAssemblyInfo;
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(string assemblyQualifiedAttributeTypeName)
        {
            return originalAssemblyInfo.GetCustomAttributes(assemblyQualifiedAttributeTypeName);
        }

        public ITypeInfo GetType(string typeName)
        {
            return GetTypes(false).FirstOrDefault(t => t.Name == typeName);
        }

        public IEnumerable<ITypeInfo> GetTypes(bool includePrivateTypes)
        {
            Console.WriteLine($"    Discovering feature files from folder {FeatureFilesFolder}");
            foreach (var featureFilePath in Directory.GetFiles(FeatureFilesFolder, "*.feature", SearchOption.AllDirectories))
            {
                var relativePath = featureFilePath.Substring(FeatureFilesFolder.Length).TrimStart(Path.DirectorySeparatorChar);
                Console.WriteLine($"      {relativePath}");
                yield return new FeatureFileTestClass(this, relativePath);
            }
        }

        public string AssemblyPath
        {
            get { return originalAssemblyInfo.AssemblyPath; }
        }

        public string Name
        {
            get { return originalAssemblyInfo.Name; }
        }

        public void Deserialize(IXunitSerializationInfo data)
        {
            string assemblyName = data.GetValue<string>("OrigAssembly");
            originalAssemblyInfo = Reflector.Wrap(Assembly.LoadFrom(assemblyName));

            //var an = new AssemblyName(assemblyName);
            //var assembly = Assembly.Load(new AssemblyName { Name = an.Name, Version = an.Version });
            //originalAssemblyInfo = Reflector.Wrap(assembly);

        }

        public void Serialize(IXunitSerializationInfo data)
        {
            data.AddValue("OrigAssembly", AssemblyPath);
        }
    }
}
