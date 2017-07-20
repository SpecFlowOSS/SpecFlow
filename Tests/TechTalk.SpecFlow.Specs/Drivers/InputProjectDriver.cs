using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NUnit.Framework;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class InputProjectDriver
    {
        private readonly string compilationFolder;

        public string CompilationFolder
        {
            get { return compilationFolder; }
        }

        public AppConfigConfigurationDriver AppConfigConfigurationDriver { get; private set; }
        public SpecFlowJsonConfigurationDriver SpecFlowJsonConfigurationDriver { get; private set; }


        public string ProjectName { get; set; }
        public string Language { get; set; }

        public List<FeatureFileInput> FeatureFiles { get; private set; }
        public FeatureFileInput CurrentFeatureFile { get; set; }

        public List<ContentFileInput> ContentFiles { get; private set; }

        public List<BindingClassInput> BindingClasses { get; private set; }
        private BindingClassInput defaultBindingClass;
        public BindingClassInput DefaultBindingClass
        {
            get
            {
                if (defaultBindingClass == null)
                {
                    defaultBindingClass = new BindingClassInput("DefaultBindings." + CodeFileExtension);
                    BindingClasses.Add(defaultBindingClass);
                }
                return defaultBindingClass;
            }
        }

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

        public string CodeFileExtension
        {
            get
            {
                switch (Language.ToLower())
                {
                    case "c#":
                        return "cs";
                    case "f#":
                        return "fs";
                    case "vb.net":
                        return "vb";
                    default:
                        throw new NotSupportedException("Language not supported: " + Language);
                }
            }
        }

        public IEnumerable<string> FrameworkAssembliesToCopy
        {
            get
            {
                switch (Language.ToLower())
                {
                    case "f#":
                        yield return "FSharp.Core.dll";
                        break;
                }
            }
        }

        public InputProjectDriver(AppConfigConfigurationDriver appConfigConfigurationDriver, SpecFlowJsonConfigurationDriver specFlowJsonConfigurationDriver)
        {
            SpecFlowJsonConfigurationDriver = specFlowJsonConfigurationDriver;
            AppConfigConfigurationDriver = appConfigConfigurationDriver;
            ProjectName = "SpecFlow.TestProject";
            Language = "C#";

            var tempDirectory = DetermineDirectoryForTestProjects();
            compilationFolder = Path.Combine(tempDirectory, "Project_" + Guid.NewGuid().ToString("D"));

            FeatureFiles = new List<FeatureFileInput>();
            ContentFiles = new List<ContentFileInput>();
            BindingClasses = new List<BindingClassInput>();

            References = new List<string>();
        }

        internal static string DetermineDirectoryForTestProjects()
        {
            var variableForProjectName = ConfigurationManager.AppSettings["testProjectFolder"] ?? "SpecFlowTestProject";
            var tempDirectory = Path.Combine(
                Path.GetTempPath(),
                Environment.ExpandEnvironmentVariables(variableForProjectName));
            return tempDirectory;
        }

        public void Reset()
        {
            ProjectName = "SpecFlow.TestProject";
            Language = "C#";

            FeatureFiles = new List<FeatureFileInput>();
            ContentFiles = new List<ContentFileInput>();
            BindingClasses = new List<BindingClassInput>();

            References = new List<string>();

            defaultBindingClass = null;
        }

        private string GetFeatureFileName()
        {
            return string.Format("Feature{0}.feature", FeatureFiles.Count + 1);
        }

        private string GetBindingFileName()
        {
            return string.Format("Binding{0}." + CodeFileExtension, BindingClasses.Count + 1);
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
            var contentFileInput = GetContentFileInput(fileName, fileContent);
            ContentFiles.Add(contentFileInput);
            return contentFileInput;
        }

        private static ContentFileInput GetContentFileInput(string fileName, string fileContent)
        {
            var file = Path.GetFileName(fileName);
            var directory = Path.GetDirectoryName(fileName);

            var isDirectoryProvided = !string.IsNullOrEmpty(directory) && directory != ".";

            var contentFileInput = isDirectoryProvided ? new ContentFileInput(file, fileContent, directory) : new ContentFileInput(file, fileContent);
            return contentFileInput;
        }

        public void AddStepBinding(ScenarioBlock scenarioBlock, string regex, string code = null)
        {
            var codeValue = GetCode(Language, code);

            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, codeValue));
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, codeValue) { ParameterType = "Table", ParameterName = "tableArg"});
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, codeValue) { ParameterType = "string", ParameterName = "mlStringArg"});
        }

        public void AddStepBinding(ScenarioBlock scenarioBlock, string regex, string csharpcode, string vbnetcode)
        {
            var codeValue = GetCode(Language, csharpcode, vbnetcode);

            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, codeValue));
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, codeValue) { ParameterType = "Table", ParameterName = "tableArg" });
            DefaultBindingClass.StepBindings.Add(new StepBindingInput(scenarioBlock, regex, codeValue) { ParameterType = "string", ParameterName = "mlStringArg" });
        }

        private string GetCode(string language, string csharpcode, string vbnetcode)
        {
            switch (language.ToLower())
            {
                case "c#":
                    return csharpcode;
                case "vb.net":
                    return vbnetcode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }

        private string GetCode(string language, string code)
        {
            if (!String.IsNullOrEmpty(code))
                return code;

            switch (language.ToLower())
            {
                case "c#":
                    return "//nop";
                case "vb.net":
                    return "'nop";
                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }

        public void AddBindingCode(string bindingCode)
        {
            DefaultBindingClass.OtherBindings.Add(bindingCode);
        }

        public void AddEventBinding(string eventType, string code, string methodName = null, int hookOrder = 10000)
        {
            methodName = methodName ?? eventType;
            AddBindingCode(string.Format(@"[{0}(Order = {4})]{1}public void {3}() {{
                Console.WriteLine(""-> hook: {3}"");
                {2}
            }}
            ", eventType, IsStaticEvent(eventType) ? "static " : "", code, methodName, hookOrder));
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
