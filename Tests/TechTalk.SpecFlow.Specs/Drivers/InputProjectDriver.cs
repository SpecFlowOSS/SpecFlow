﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class InputProjectDriver
    {
        private readonly string compilationFolder;

        public string CompilationFolder
        {
            get { return compilationFolder; }
        }

        public SpecFlowConfigurationDriver ConfigurationDriver { get; private set; }

        public string ProjectName { get; set; }

        public List<FeatureFileInput> FeatureFiles { get; private set; }
        public FeatureFileInput CurrentFeatureFile { get; set; }

        public List<ContentFileInput> ContentFiles { get; private set; }

        public List<BindingClassInput> BindingClasses { get; private set; }
        public BindingClassInput DefaultBindingClass { get; set; }

        public List<string> References { get; private set; }

        public string CompiledAssemblyPath
        {
            get { return Path.Combine(DeploymentFolder, ProjectName + ".dll"); }
        }

        public string DeploymentFolder
        {
            get { return Path.Combine(compilationFolder, "bin", "Debug"); }
        }
        public string ProjectFilePath { get; set; }

        public InputProjectDriver(SpecFlowConfigurationDriver configurationDriver)
        {
            ConfigurationDriver = configurationDriver;
            ProjectName = "SpecFlow.TestProject";

            compilationFolder = Path.Combine(Path.GetTempPath(), Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["testProjectFolder"]));

            FeatureFiles = new List<FeatureFileInput>();
            ContentFiles = new List<ContentFileInput>();
            BindingClasses = new List<BindingClassInput>();

            DefaultBindingClass = new BindingClassInput("DefaultBindings.cs");
            BindingClasses.Add(DefaultBindingClass);

            References = new List<string>();
        }

        private string GetFeatureFileName()
        {
            return string.Format("Feature{0}.sfeature", FeatureFiles.Count + 1);
        }

        private string GetBindingFileName()
        {
            return string.Format("Binding{0}.cs", BindingClasses.Count + 1);
        }

        public void AddFeatureFile(string featureFileText, string featureFileName = null)
        {
            featureFileName = featureFileName ?? GetFeatureFileName();
            var featureFile = new FeatureFileInput(featureFileName, featureFileText);
            FeatureFiles.Add(featureFile);
            CurrentFeatureFile = featureFile;
        }

        public ContentFileInput AddContentFile(string fileName, string fileContent)
        {
            var contentFileInput = new ContentFileInput(fileName, fileContent);
            ContentFiles.Add(contentFileInput);
            return contentFileInput;
        }

        public void AddStepBinding(ScenarioBlock scenarioBlock, string regex, string code = "//nop")
        {
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, code));
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, code) { Parameters = "Table tableArg"});
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, code) { Parameters = "string mlStringArg"});
        }

        public void AddBindingCode(string bindingCode)
        {
            DefaultBindingClass.OtherBindings.Add(bindingCode);
        }

        public void AddEventBinding(string eventType, string code, string methodName = null)
        {
            methodName = methodName ?? eventType;
            AddBindingCode(string.Format(@"[{0}]{1}public void {3}() {{
                Console.WriteLine(""-> hook: {3}"");
                {2}
            }}
            ", eventType, IsStaticEvent(eventType) ? "static " : "", code, methodName));
        }

        private bool IsStaticEvent(string eventType)
        {
            return
                eventType == "BeforeFeature" ||
                eventType == "AfterFeature" ||
                eventType == "BeforeTestRun" ||
                eventType == "AfterTestRun";
        }

        public void AddRawBindingClass(string rawBindingClass)
        {
            BindingClasses.Add(new BindingClassInput(GetBindingFileName(), rawBindingClass, "."));
        }
    }
}
