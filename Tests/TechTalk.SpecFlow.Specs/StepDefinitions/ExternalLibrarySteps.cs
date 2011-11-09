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
            inputProjectDriver.ProjectName = libraryName;
        }

        [Given(@"the following step definition in the external library")]
        public void GivenTheFollowingStepDefinitionInTheExternalLibrary(string stepDefinition)
        {
            Given("the following step definition", stepDefinition);
        }

        [Given(@"there is a SpecFlow project with a reference to the external library")]
        public void GivenThereIsASpecFlowProjectWithAReferenceToTheExternalLibrary()
        {
            var project = projectGenerator.GenerateProject(inputProjectDriver);
            projectCompiler.Compile(project);

            var libName = inputProjectDriver.CompiledAssemblyPath;
            var savedLibPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(libName));
            File.Copy(libName, savedLibPath, true);

            inputProjectDriver.DefaultBindingClass.OtherBindings.Clear();
            inputProjectDriver.DefaultBindingClass.StepBindings.Clear();
            inputProjectDriver.ProjectName = "SpecFlow.TestProject";
            inputProjectDriver.References.Add(savedLibPath);
        }
    }
}
