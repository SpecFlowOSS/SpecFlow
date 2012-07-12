using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings.Discovery
{
    internal class VsBindingMethodLocator
    {
        private readonly VsBindingReflectionFactory bindingReflectionFactory = new VsBindingReflectionFactory();

        public CodeFunction FindCodeFunction(VsProjectScope projectScope, IBindingMethod bindingMethod)
        {
            var project = projectScope.Project;

            var function = FindCodeFunction(project, bindingMethod);
            if (function != null)
                return function;

            var specFlowProject = projectScope.SpecFlowProjectConfiguration;
            if (specFlowProject != null)
            {
                foreach (var assemblyName in specFlowProject.RuntimeConfiguration.AdditionalStepAssemblies)
                {
                    string simpleName = assemblyName.Split(new[] { ',' }, 2)[0];

                    var stepProject = VsxHelper.FindProjectByAssemblyName(project.DTE, simpleName);
                    if (stepProject != null)
                    {
                        function = FindCodeFunction(stepProject, bindingMethod);
                        if (function != null)
                            return function;
                    }
                }
            }

            return null;
        }

        private CodeFunction FindCodeFunction(Project project, IBindingMethod bindingMethod)
        {
            return GetClassesIncludingPartialClasses(project)
                .Where(c => c.FullName == bindingMethod.Type.FullName)
                .SelectMany(c => c.GetFunctions()).FirstOrDefault(
                    f => f.Name == bindingMethod.Name && bindingMethod.MethodEquals(bindingReflectionFactory.CreateBindingMethod(f)));
        }

        private IEnumerable<CodeClass> GetClassesIncludingPartialClasses(Project project)
        {
            foreach (CodeClass bindingClassWithBindingAttribute in VsxHelper.GetClasses(project))
            {
                yield return bindingClassWithBindingAttribute;

                CodeClass2 bindingClassIncludingParts = bindingClassWithBindingAttribute as CodeClass2;
                if (bindingClassIncludingParts != null)
                {
                    foreach (CodeClass2 currentBindingPartialClass in bindingClassIncludingParts.Parts)
                    {
                        yield return currentBindingPartialClass;
                    }
                }
            }
        }
        
    }
}