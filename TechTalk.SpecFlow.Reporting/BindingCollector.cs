using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Reporting
{
    [Serializable]
    public class FilePosition
    {
        public string FilePath { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }

    [Serializable]
    public class BindingInfo
    {
        public string MethodReference { get; set; }
        public FilePosition FilePosition { get; set; }

        public string BindingType { get; set; }
        public Regex Regex { get; set; }
        public string[] ParameterNames { get; set; }
        public bool HasMultilineTextArg { get; set; }
        public bool HasTableArg { get; set; }
    }

    public class BindingCollector : MarshalByRefObject
    {
        public void BuildBindingsFromAssembly(Assembly assembly, List<BindingInfo> bindings)
        {
            foreach (Type type in assembly.GetTypes())
            {
                Attribute bindingAttr = Attribute.GetCustomAttribute(type, typeof(BindingAttribute));
                if (bindingAttr == null)
                    continue;

                BuildBindingsFromType(type, bindings);
            }
        }

        private void BuildBindingsFromType(Type type, List<BindingInfo> bindings)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var scenarioStepAttrs = Attribute.GetCustomAttributes(method, typeof(StepDefinitionBaseAttribute));
                if (scenarioStepAttrs != null)
                    foreach (var scenarioStepAttr in scenarioStepAttrs)
                    {
                        BuildStepBindingFromMethod(method, scenarioStepAttr, bindings);
                    }

//                var bindingEventAttrs = Attribute.GetCustomAttributes(method, typeof(BindingEventAttribute));
//                if (bindingEventAttrs != null)
//                    foreach (BindingEventAttribute bindingEventAttr in bindingEventAttrs)
//                    {
//                        BuildEventBindingFromMethod(method, bindingEventAttr);
//                    }
            }
        }

        private void BuildStepBindingFromMethod(MethodInfo method, Attribute scenarioStepAttr, List<BindingInfo> bindings)
        {
            Regex regex = new Regex("^" + scenarioStepAttr.GetProperty<string>("Regex") + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            var parameters = method.GetParameters();

            var hasTableArg = parameters.Length == 0 
                                  ? false
                                  : parameters[parameters.Length - 1].ParameterType.Name == "Table";

            BindingInfo bindingInfo = new BindingInfo
                                          {
                                              BindingType = GetBindingType(scenarioStepAttr),
                                              Regex = regex,
                                              MethodReference = String.Format("{0}.{1}({2})",
                                                                              method.ReflectedType.FullName, method.Name, String.Join(", ", parameters.Select(pi => pi.ParameterType.Name).ToArray())),
                                              ParameterNames = parameters.Select(pi => pi.Name).ToArray(),
                                              HasMultilineTextArg = false, //TODO
                                              HasTableArg = hasTableArg
                                          };

            bindings.Add(bindingInfo);
        }

        private string GetBindingType(Attribute scenarioStepAttr)
        {
            var attrName = scenarioStepAttr.GetType().Name;
            return attrName.Remove(attrName.Length - "Attribute".Length);
        }

        public List<BindingInfo> CollectBindings(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            var bindings = new List<BindingInfo>();

            BuildBindingsFromAssembly(assembly, bindings);

            return bindings;
        }

        internal static List<BindingInfo> CollectBindings(SpecFlowProject specFlowProject, string basePath)
        {
            var reportingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = reportingFolder;
            AppDomain appDomain = AppDomain.CreateDomain("CollectBindings", null, appDomainSetup);

            AssemblyServices.SubscribeAssemblyResolve(appDomain, basePath);
            
            BindingCollector bindingCollector = 
                (BindingCollector)appDomain.CreateInstanceAndUnwrap(
                                      Assembly.GetExecutingAssembly().GetName().FullName,
                                      typeof(BindingCollector).FullName);

            var stepAssemblies = GetStepAssemblies(specFlowProject);

            List<BindingInfo> bindings =
                stepAssemblies.SelectMany(bindingCollector.CollectBindings).ToList();

            AppDomain.Unload(appDomain);
            return bindings;
        }

        private static IEnumerable<string> GetStepAssemblies(SpecFlowProject specFlowProject)
        {
            yield return specFlowProject.ProjectSettings.AssemblyName;

            foreach (var stepAssembly in specFlowProject.Configuration.SpecFlowConfiguration.AdditionalStepAssemblies)
            {
                yield return stepAssembly;
            }
        }
    }
}