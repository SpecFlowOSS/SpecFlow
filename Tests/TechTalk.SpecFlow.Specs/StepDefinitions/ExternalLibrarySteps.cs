using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.MsBuild;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExternalLibrarySteps : Steps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly ProjectGenerator projectGenerator;
        private readonly ProjectCompiler projectCompiler;

        public ExternalLibrarySteps(InputProjectDriver inputProjectDriver, ProjectGenerator projectGenerator, ProjectCompiler projectCompiler)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.projectGenerator = projectGenerator;
            this.projectCompiler = projectCompiler;
        }

        [Given(@"there is an external class library project '(.*)'")]
        public void GivenThereIsAnExternalClassLibraryProject(string libraryName)
        {
            GivenThereIsAnExternalClassLibraryProject("C#", libraryName);
        }

        [Given(@"there is an external (.+) class library project '(.*)'")]
        public void GivenThereIsAnExternalClassLibraryProject(string language, string libraryName)
        {
            inputProjectDriver.ProjectName = libraryName;
            inputProjectDriver.Language = language;
        }

        [Given(@"the following step definition in the external library")]
        public void GivenTheFollowingStepDefinitionInTheExternalLibrary(string stepDefinition)
        {
            Given("the following step definition", stepDefinition);
        }

        [Given(@"there is a (.+) SpecFlow project with a reference to the external library")]
        public void GivenThereIsASpecFlowProjectWithAReferenceToTheExternalLibrary(string language)
        {
            var project = projectGenerator.GenerateProject(inputProjectDriver);
            projectCompiler.Compile(project);

            List<string> assembliesToReference = new List<string>();

            var libName = inputProjectDriver.CompiledAssemblyPath;
            var savedLibPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(libName));
            File.Copy(libName, savedLibPath, true);
            assembliesToReference.Add(savedLibPath);

            foreach (var assemblyFileName in inputProjectDriver.FrameworkAssembliesToCopy)
            {
                var originalAssemblyPath = Path.Combine(inputProjectDriver.DeploymentFolder, assemblyFileName);
                var savedAssemblyPath = Path.Combine(Path.GetTempPath(), assemblyFileName);
                File.Copy(originalAssemblyPath, savedAssemblyPath, true);
                assembliesToReference.Add(savedAssemblyPath);
            }

            inputProjectDriver.Reset();
            inputProjectDriver.ProjectName = "SpecFlow.TestProject";
            inputProjectDriver.Language = language;
            foreach (var assemblyPath in assembliesToReference)
            {
                inputProjectDriver.References.Add(assemblyPath);
            }
        }

        [Given(@"there is a SpecFlow project with a reference to the external library")]
        public void GivenThereIsASpecFlowProjectWithAReferenceToTheExternalLibrary()
        {
            GivenThereIsASpecFlowProjectWithAReferenceToTheExternalLibrary("C#");
        }
    }
}
