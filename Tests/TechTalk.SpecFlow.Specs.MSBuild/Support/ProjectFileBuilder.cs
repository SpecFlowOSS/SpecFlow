using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TechTalk.SpecFlow.Specs.MSBuild.Support
{
    public class ProjectFileBuilder
    {
        private readonly List<ImportElement> _propFileImports = new List<ImportElement>();

        private readonly List<ImportElement> _targetFileImports = new List<ImportElement>();

        private readonly List<KeyValuePair<string, string>> _properties = new List<KeyValuePair<string, string>>();

        private readonly List<PackageReferenceElement> _packageReferences = new List<PackageReferenceElement>();

        private readonly List<ProjectReferenceElement> _projectReferences = new List<ProjectReferenceElement>();

        private static readonly string Namespace =
#if NETCOREAPP2_2
                null;
#else
                "http://schemas.microsoft.com/developer/msbuild/2003";
#endif

        public string RootNamespace { get; set; }

        public string AssemblyName { get; set; }

        public string RepoRoot { get; set; }

        public void AddPropFileImport(string path, string condition = null)
        {
            _propFileImports.Add(new ImportElement { Project = path, Condition = condition });
        }

        public void AddTargetFileImport(string path, string condition = null)
        {
            _targetFileImports.Add(new ImportElement { Project = path, Condition = condition });
        }

        public void AddPropertyValue(string name, string value)
        {
            _properties.Add(new KeyValuePair<string, string>(name, value));
        }

        public void AddPackageReference(string package, string version, string privateAssets = null, string includeAssets = null)
        {
            _packageReferences.Add(
                new PackageReferenceElement
                {
                    Include = package,
                    Version = version,
                    PrivateAssets = privateAssets,
                    IncludeAssets = includeAssets
                });
        }

        public void AddProjectReference(string path, string additionalProperties = null)
        {
            _projectReferences.Add(new ProjectReferenceElement { Include = path, AdditionalProperties = additionalProperties });
        }

        public void WriteToFile(string path)
        {
            var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " };
            using (var writer = XmlWriter.Create(path, writerSettings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Project", Namespace);

#if !NETCOREAPP2_2
                // Add tools version attribute.
                writer.WriteAttributeString("ToolsVersion", "15.0");

#else
                // Add SDK for project.
                writer.WriteAttributeString("SDK", "Microsoft.NET.Sdk");
#endif
                
                // Add the path to the repository root (assists with imports).
                if (RepoRoot != null)
                {
                    WriteProperties(writer, new[] { new KeyValuePair<string, string>("RepoRoot", RepoRoot) });
                }

                WriteImports(writer, _propFileImports);

                WriteProperties(writer, _properties);

#if !NETCOREAPP2_2
                WriteClassicProperties(writer);
#endif
                WritePackageReferences(writer);

                WriteProjectReferences(writer);

                WriteImports(writer, _targetFileImports);

                writer.WriteEndElement();

                writer.Flush();
            }
        }

        private void WriteProjectReferences(XmlWriter writer)
        {
            if (_projectReferences.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("ItemGroup", Namespace);

            foreach (var reference in _projectReferences)
            {
                writer.WriteStartElement("ProjectReference", Namespace);

                writer.WriteAttributeString("Include", reference.Include);

                if (reference.AdditionalProperties != null)
                {
                    writer.WriteAttributeString("AdditionalProperties", reference.AdditionalProperties);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WritePackageReferences(XmlWriter writer)
        {
            if (_packageReferences.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("ItemGroup", Namespace);

            foreach (var reference in _packageReferences)
            {
                writer.WriteStartElement("PackageReference", Namespace);

                writer.WriteAttributeString("Include", reference.Include);
                writer.WriteAttributeString("Version", reference.Version);

                if (reference.PrivateAssets != null)
                {
                    writer.WriteElementString("PrivateAssets", Namespace, reference.PrivateAssets);
                }

                if (reference.IncludeAssets != null)
                {
                    writer.WriteElementString("IncludeAssets", Namespace, reference.IncludeAssets);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WriteClassicProperties(XmlWriter writer)
        {
            writer.WriteStartElement("PropertyGroup", Namespace);

            WriteProperty(writer, "Configuration", "Debug", " '$(Configuration)' == '' ");
            WriteProperty(writer, "Platform", "AnyCPU", " '$(Platform)' == '' ");
            WriteProperty(writer, "ProjectGuid", Guid.NewGuid().ToString());
            WriteProperty(writer, "OutputType", "Library");
            WriteProperty(writer, "AppDesignerFolder", "Properties");
            WriteProperty(writer, "RootNamespace", RootNamespace);
            WriteProperty(writer, "AssemblyName", AssemblyName);
            WriteProperty(writer, "TargetFrameworkVersion", "v4.7.1");
            WriteProperty(writer, "FileAlignment", "512");
            WriteProperty(writer, "Deterministic", "true");

            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup", Namespace);
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ");

            WriteProperty(writer, "DebugSymbols", "true");
            WriteProperty(writer, "DebugType", "full");
            WriteProperty(writer, "Optimize", "false");
            WriteProperty(writer, "OutputPath", @"bin\Debug\");
            WriteProperty(writer, "DefineConstants", "DEBUG;TRACE");
            WriteProperty(writer, "ErrorReport", "prompt");
            WriteProperty(writer, "WarningLevel", "4");

            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup", Namespace);
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ");

            WriteProperty(writer, "DebugType", "pdbonly");
            WriteProperty(writer, "Optimize", "true");
            WriteProperty(writer, "OutputPath", @"bin\Release\");
            WriteProperty(writer, "DefineConstants", "TRACE");
            WriteProperty(writer, "ErrorReport", "prompt");
            WriteProperty(writer, "WarningLevel", "4");

            writer.WriteEndElement();
        }

        private void WriteProperty(XmlWriter writer, string name, string value, string condition = null)
        {
            writer.WriteStartElement(name, Namespace);

            if (condition != null)
            {
                writer.WriteAttributeString("Condition", condition);
            }

            writer.WriteValue(value);

            writer.WriteEndElement();
        }

        private void WriteProperties(XmlWriter writer, IReadOnlyCollection<KeyValuePair<string, string>> properties)
        {
            if (properties.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("PropertyGroup", Namespace);

            foreach (var property in properties)
            {
                writer.WriteStartElement(property.Key, Namespace);
                writer.WriteValue(property.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WriteImports(XmlWriter writer, IReadOnlyCollection<ImportElement> imports)
        {
            if (imports.Count == 0)
            {
                return;
            }

            foreach (var import in imports)
            {
                writer.WriteStartElement("Import", Namespace);

                writer.WriteAttributeString("Project", import.Project);

                if (import.Condition != null)
                {
                    writer.WriteAttributeString("Condition", import.Condition);
                }

                writer.WriteEndElement();
            }
        }

        private class ImportElement
        {
            public string Project { get; set; }

            public string Condition { get; set; }
        }

        private class PackageReferenceElement
        {
            public string Include { get; set; }

            public string Version { get; set; }

            public string PrivateAssets { get; set; }

            public string IncludeAssets { get; set; }
        }

        private class ProjectReferenceElement
        {
            public string Include { get; set; }

            public string AdditionalProperties { get; set; }
        }
    }
}
