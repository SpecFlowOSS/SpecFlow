using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.FSharp.Core;
using SpecFlow.TestProjectGenerator.Helpers;
using SpecFlow.TestProjectGenerator.Inputs;
using SpecFlow.TestProjectGenerator.ProgramLanguageDrivers;

namespace SpecFlow.TestProjectGenerator
{
    public class InputProjectDriver
    {
        private readonly Folders _folders;
        private readonly AppConfigDriver _appConfigDriver;
        private IProgramLanguageInputProjectDriver _programLanguageInputProjectDriver;
        public Guid ProjectGuid { get; } = Guid.NewGuid();
        private ProgrammingLanguage _programmingLanguage;
        private string _solutionFolder;

        public string ProjectFolder => Path.Combine(SolutionFolder, "Project");
        public string ProjectName => $"TestProject_{ProjectGuid:N}";
        public string ProjectFileName => _programLanguageInputProjectDriver.GetProjectFileName(ProjectName);
        public string ProjectPath => Path.Combine(ProjectFolder, ProjectFileName);

        public string SolutionFolder => _solutionFolder;

        public string SolutionName => ProjectName;
        public string SolutionFileName => $"{SolutionName}.sln";
        public string SolutionPath => Path.Combine(SolutionFolder, SolutionFileName);
        public string PackageFolderPath => Path.Combine(SolutionFolder, "packages");

        public List<FeatureFileInput> FeatureFiles { get; private set; } = new List<FeatureFileInput>();
        public FeatureFileInput CurrentFeatureFile { get; set; }

        public List<ContentFileInput> ContentFiles { get; private set; } = new List<ContentFileInput>();
        public ContentFileInput CurrentSpecRunConfigFile { get; set; }

        public List<BindingClassInput> BindingClasses { get; private set; } = new List<BindingClassInput>();
        //public BindingClassInput CurrentBindingClass { get; set; }
        public BindingClassInput DefaultBindingClass { get; set; }

        public List<CodeFileInput> CodeFileInputs { get; private set; } = new List<CodeFileInput>();

        public UnitTestProvider UnitTestProvider { get; set; } = UnitTestProvider.SpecRun;

        public ProgrammingLanguage ProgrammingLanguage
        {
            get { return _programmingLanguage; }
            set
            {
                _programmingLanguage = value;
                SwitchProgramLanguage(value);
            }
        }

        public string NetFrameworkVersion { get; set; } = "v4.5";

        public string AdditionalSpecFlowPlugins { get; set; } = "";

        public string AdditionalSpecFlowSettings { get; set; } = "";

        public List<string> AdditionalReferences { get; private set; } = new List<string>();

        public string CompiledAssemblyPath => Path.Combine(DeploymentFolder, "TestProject.dll");

        public string DeploymentFolder => Path.Combine(ProjectFolder, "bin", "Debug");
        public string TestingFrameworkReference { get; set; }


        public InputProjectDriver(Folders folders, AppConfigDriver appConfigDriver)
        {
            TestingFrameworkReference = "<Reference Include=\"Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL\" />";
            
            _folders = folders;
            _appConfigDriver = appConfigDriver;

            SwitchProgramLanguage(ProgrammingLanguage.CSharp);

            SetProjectRootFolder(FSharpOption<string>.None);

            GetAndAddDefaultBindingClass();
        }

        private void GetAndAddDefaultBindingClass()
        {
            BindingClasses.Clear();
            DefaultBindingClass = _programLanguageInputProjectDriver.GetDefaultBindingClass();
            BindingClasses.Add(DefaultBindingClass);
        }

        private void SwitchProgramLanguage(ProgrammingLanguage value)
        {
            switch (value)
            {
                case ProgrammingLanguage.CSharp:
                    _programLanguageInputProjectDriver = new CSharpProgramLanguageInputProjectDriver();
                    break;
                case ProgrammingLanguage.VB:
                    _programLanguageInputProjectDriver = new VBNetProgramLanguageInputProjectDriver();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            _programmingLanguage = value;

            GetAndAddDefaultBindingClass();
        }

        internal string DetermineDirectoryForTestProjects()
        {
            return Path.Combine(_folders.TestProjectRootFolder, Environment.ExpandEnvironmentVariables(_appConfigDriver.ProjectName));
        }

        private string GetFeatureFileName()
        {
            return $"Feature{FeatureFiles.Count + 1}.feature";
        }

        public void AddFeatureFile(string featureFileText)
        {
            var featureFile = new FeatureFileInput(GetFeatureFileName(), featureFileText);
            FeatureFiles.Add(featureFile);
            CurrentFeatureFile = featureFile;
        }

        public ContentFileInput AddContentFile(string fileName, string fileContent)
        {
            var contentFileInput = new ContentFileInput(fileName, fileContent);
            var existingContentFiles = ContentFiles.Where(cf => cf.FileName == fileName).ToList();
            if (existingContentFiles.Any())
            {
                existingContentFiles.ForEach(i => ContentFiles.Remove(i));
            }

            ContentFiles.Add(contentFileInput);
            return contentFileInput;
        }

        public void AddSpecRunConfigFile(string fileName, string fileContent)
        {
            CurrentSpecRunConfigFile = AddContentFile(fileName, fileContent);
        }

        public void AddStepBinding(string scenarioBlock, string regex, string csharpcode = "//nop", string vbnetcode = "'nop")
        {
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, csharpcode, vbnetcode));
        }

        public void AddBindingCode(string bindingCode)
        {
            DefaultBindingClass.OtherBindings.Add(bindingCode);
        }

        public void AddEventBinding(string eventType, string code)
        {
            AddBindingCode(GetBindingCode(eventType, code));
        }

        private string GetBindingCode(string eventType, string code)
        {
            return _programLanguageInputProjectDriver.GetBindingCode(eventType, code);
        }
        

        public void SetTestAdapterPath(string testAdapterPath)
        {
            _folders.VSAdapterFolder = Path.Combine(SolutionFolder, testAdapterPath);
        }


        public void SetProjectRootFolder(FSharpOption<string> rootFolder)
        {
            var tempDirectory = DetermineDirectoryForTestProjects();

            if (rootFolder.IsSome())
            {
                tempDirectory = Path.Combine(tempDirectory, rootFolder.Value);
            }

            _solutionFolder = Path.Combine(tempDirectory, "Project_" + ProjectGuid.ToString("D"));
        }

        public void UseNUnitVersionAsTestingFramework(string nunitVersion)
        {
            if (nunitVersion == "2.6.4")
            {
                TestingFrameworkReference = "<Reference Include=\"nunit.framework\">" +
                                            $"<HintPath>{Path.Combine(PackageFolderPath, $"NUnit.{nunitVersion}", "lib", "nunit.framework.dll")}</HintPath>" +
                                            "</Reference>";
            }
            else
            {
                TestingFrameworkReference = "<Reference Include=\"nunit.framework\">" +
                                            $"<HintPath>{Path.Combine(PackageFolderPath, $"NUnit.{nunitVersion}", "lib", "net45", "nunit.framework.dll")}</HintPath>" +
                                            "</Reference>";
            }




            TestingFrameworkPackage = $"<package id=\"NUnit\" version=\"{nunitVersion}\" targetFramework=\"net45\" />";
        }

        public void AddBindingClass(string fileName, string code)
        {
            CodeFileInputs.Add(new CodeFileInput(fileName, ".", code));
        }

        public string TestingFrameworkPackage { get; set; }
    }
}
