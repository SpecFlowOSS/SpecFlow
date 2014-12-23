using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public class ProjectStepDefinitions
    {
        public string ProjectName { get; set; }
        public List<FileStepDefinitions> FileStepDefinitions { get; set; }
    }

    public class StepDefinitionBindingItem
    {
        public IBindingMethod Method { get; set; }
        public StepDefinitionType StepDefinitionType { get; set; }
        public Regex Regex { get; set; }
        public BindingScope BindingScope { get; set; }

        static public StepDefinitionBindingItem FromStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            return new StepDefinitionBindingItem()
                       {
                           Method = stepDefinitionBinding.Method,
                           StepDefinitionType =  stepDefinitionBinding.StepDefinitionType,
                           Regex = stepDefinitionBinding.Regex,
                           BindingScope = stepDefinitionBinding.BindingScope
                       };
        }

        public StepDefinitionBinding ToStepDefinitionBinding()
        {
            return new StepDefinitionBinding(StepDefinitionType, Regex, Method, BindingScope);
        }
    }

    public class FileStepDefinitions
    {
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<StepDefinitionBindingItem> StepDefinitions { get; set; }
    }

    public class FeatureSteps
    {
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Feature Feature { get; set; }
        public Version GeneratorVersion { get; set; }
    }

    public class StepMap
    {
        public const int CURRENT_VERSION = 3;

        public static StepMap CreateStepMap(CultureInfo defaultLanguage)
        {
            return new StepMap()
                       {
                           FileVersion = CURRENT_VERSION,
                           IntegrationVersion = GetIntegrationVersion(),
                           DefaultLanguage = defaultLanguage
                       };
        }

        private static Version GetIntegrationVersion()
        {
            return typeof(StepMap).Assembly.GetName().Version;
        }

        public int FileVersion { get; set; }
        public Version IntegrationVersion { get; set; }
        public CultureInfo DefaultLanguage { get; set; }
        public List<ProjectStepDefinitions> ProjectStepDefinitions { get; set; }
        public List<FeatureSteps> FeatureSteps { get; set; }

        private int FeatureFileCount
        {
            get { return FeatureSteps.Count; }
        }

        private int StepDefinitionCount
        {
            get { return ProjectStepDefinitions.Sum(psd => psd.FileStepDefinitions.Sum(fsd => fsd.StepDefinitions.Count)); }
        }

        public void SaveToFile(string fileName, IIdeTracer tracer)
        {
            try
            {
                string folder = Path.GetDirectoryName(fileName);
                Debug.Assert(folder != null);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var serializer = new StepMapSetializer();

                string tempFileName = fileName + ".new";

                using (var writer = new StreamWriter(tempFileName, false, Encoding.UTF8))
                {
                    serializer.Serialize(writer, this);
                }

                File.Copy(tempFileName, fileName, true);
                File.Delete(tempFileName);

                tracer.Trace(string.Format("StepMap with {0} feature files and {1} step definitions saved", FeatureFileCount, StepDefinitionCount), GetType().Name);
            }
            catch (Exception saveException)
            {
                tracer.Trace(string.Format("StepMap saving error: {0}", saveException), typeof(StepMap).Name);
            }
        }

        public static StepMap LoadFromFile(string fileName, IIdeTracer tracer)
        {
            try
            {
                var serializer = new StepMapSetializer();

                using (var reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    var stepMap = serializer.Deserialize(reader);

                    if (stepMap.FileVersion != CURRENT_VERSION)
                    {
                        tracer.Trace(string.Format("StepMap has wrong file version"), typeof(StepMap).Name);
                        return null;
                    }

                    if (stepMap.IntegrationVersion != GetIntegrationVersion())
                    {
                        tracer.Trace(string.Format("StepMap file is generated by another SpecFlow version."), typeof(StepMap).Name);
                        return null;
                    }

                    tracer.Trace(string.Format("StepMap with {0} feature files and {1} step definitions loaded", stepMap.FeatureFileCount, stepMap.StepDefinitionCount), typeof(StepMap).Name);

                    return stepMap;
                }
            }
            catch(Exception loadException)
            {
                tracer.Trace(string.Format("StepMap loading error: {0}", loadException), typeof(StepMap).Name);
                return null;
            }
        }
    }
}
