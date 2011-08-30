using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{

    [Serializable]
    public class BindingInfo
    {
        public Regex Regex { get; set; }
    }

    public class BindingCollector : MarshalByRefObject
    {
        private readonly Type bindingAttributeType;
        private readonly Type scenarioStepAttributeType;

        public BindingCollector()
        {
            Assembly specFlowRuntime = Assembly.Load("TechTalk.SpecFlow");
            this.bindingAttributeType = specFlowRuntime.GetType("TechTalk.SpecFlow.BindingAttribute", true);
            this.scenarioStepAttributeType = specFlowRuntime.GetType("TechTalk.SpecFlow.ScenarioStepAttribute", true);
        }

        public void BuildBindingsFromAssembly(Assembly assembly, List<BindingInfo> bindings)
        {
            foreach (Type type in assembly.GetTypes())
            {
                Attribute bindingAttr = Attribute.GetCustomAttribute(type, bindingAttributeType);
                if (bindingAttr == null)
                    continue;

                BuildBindingsFromType(type, bindings);
            }
        }

        private void BuildBindingsFromType(Type type, List<BindingInfo> bindings)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var scenarioStepAttrs = Attribute.GetCustomAttributes(method, scenarioStepAttributeType);
                if (scenarioStepAttrs != null)
                    foreach (var scenarioStepAttr in scenarioStepAttrs)
                    {
                        BuildStepBindingFromMethod(scenarioStepAttr, bindings);
                    }

                //                var bindingEventAttrs = Attribute.GetCustomAttributes(method, typeof(BindingEventAttribute));
                //                if (bindingEventAttrs != null)
                //                    foreach (BindingEventAttribute bindingEventAttr in bindingEventAttrs)
                //                    {
                //                        BuildEventBindingFromMethod(method, bindingEventAttr);
                //                    }
            }
        }

        private void BuildStepBindingFromMethod(Attribute scenarioStepAttr, List<BindingInfo> bindings)
        {
            Regex regex = new Regex("^" + scenarioStepAttr.GetProperty<string>("Regex") + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

            BindingInfo bindingInfo = new BindingInfo
            {
                Regex = regex,
            };

            bindings.Add(bindingInfo);
        }

        public List<BindingInfo> CollectBindings(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            var bindings = new List<BindingInfo>();

            BuildBindingsFromAssembly(assembly, bindings);

            return bindings;
        }

        internal static List<BindingInfo> CollectBindings(TechTalk.SpecFlow.Generator.Project.SpecFlowProject specFlowProject, string basePath)
        {
            List<BindingInfo> bindings = new List<BindingInfo>();
            var reportingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = reportingFolder;
            AppDomain appDomain = AppDomain.CreateDomain("CollectBindings", null, appDomainSetup);

            AssemblyServices.SubscribeAssemblyResolve(appDomain, basePath);

            try
            {
                BindingCollector bindingCollector =
                    (BindingCollector)appDomain.CreateInstanceAndUnwrap(
                        Assembly.GetExecutingAssembly().GetName().FullName,
                        typeof(BindingCollector).FullName);

                bindings.AddRange(bindingCollector.CollectBindings(specFlowProject.ProjectSettings.AssemblyName));

                AppDomain.Unload(appDomain);
                return bindings;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    static class ReflectionHelperExtensions
    {
        public static T GetProperty<T>(this object source, string propertyName)
        {
            return (T)source.GetType().GetProperty(propertyName).GetValue(source, null);
        }
    }
}
