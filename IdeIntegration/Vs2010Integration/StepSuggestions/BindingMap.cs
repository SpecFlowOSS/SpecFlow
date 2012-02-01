using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public class ProjectStepDefinitions
    {
        public string ProjectName { get; set; }
        public List<FileStepDefinitions> FileStepDefinitions { get; set; }
    }

    public class FileStepDefinitions
    {
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<StepBindingNew> StepDefinitions { get; set; }
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
        public const int CURRENT_VERSION = 2;

        public static StepMap CreateStepMap(CultureInfo defaultLanguage)
        {
            return new StepMap()
                       {
                           FileVersion = CURRENT_VERSION,
                           DefaultLanguage = defaultLanguage.Name
                       };
        }

        public int FileVersion { get; set; }
        public string DefaultLanguage { get; set; }
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
                var serializer = CreateSerializer();

                string tempFileName = fileName + "new";

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
                var serializer = CreateSerializer();

                using (var reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    var stepMap = (StepMap)serializer.Deserialize(reader, typeof(StepMap));

                    if (stepMap.FileVersion != CURRENT_VERSION)
                    {
                        tracer.Trace(string.Format("StepMap has wrong file version"), typeof(StepMap).Name);
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

        class CustomContractResolver : DefaultContractResolver
        {
            public override JsonContract ResolveContract(Type type)
            {
                if (type == typeof(IBindingMethod))
                    return base.ResolveContract(typeof(BindingMethod));

                if (type == typeof(IBindingParameter))
                    return base.ResolveContract(typeof(BindingParameter));

                if (type == typeof(IBindingType))
                    return base.ResolveContract(typeof(BindingReflectionType));

                return base.ResolveContract(type);
            }
        }

        private static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new CustomContractResolver();
            return serializer;
        }
    }
}
