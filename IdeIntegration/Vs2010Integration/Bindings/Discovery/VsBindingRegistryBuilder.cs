using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.IdeIntegration.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings.Discovery
{
    public class VsBindingRegistryBuilder
    {
        private readonly VsBindingReflectionFactory bindingReflectionFactory = new VsBindingReflectionFactory();

        public IEnumerable<IStepDefinitionBinding> GetBindingsFromProjectItem(ProjectItem projectItem)
        {
            var bindingProcessor = new IdeBindingSourceProcessor();
            ProcessBindingsFromProjectItem(projectItem, bindingProcessor);
            return bindingProcessor.ReadStepDefinitionBindings();
        }

        private void ProcessBindingsFromProjectItem(ProjectItem projectItem, IdeBindingSourceProcessor bindingSourceProcessor)
        {
            foreach (CodeClass codeClass in VsxHelper.GetClasses(projectItem))
            {
                CodeClass2 bindingClassIncludingParts = codeClass as CodeClass2;
                if (bindingClassIncludingParts == null)
                {
                    ProcessCodeClass(codeClass, bindingSourceProcessor);
                }
                else
                {
                    foreach (CodeClass2 partialClassParts in bindingClassIncludingParts.Parts)
                    {
                        ProcessCodeClass(partialClassParts, bindingSourceProcessor);
                    }
                }
            }
        }

        private void ProcessCodeClass(CodeClass codeClass, IdeBindingSourceProcessor bindingSourceProcessor)
        {
            var filteredAttributes = codeClass.Attributes.Cast<CodeAttribute2>().Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.FullName)).ToArray();
            if (!bindingSourceProcessor.PreFilterType(filteredAttributes.Select(attr => attr.FullName)))
                return;

            var bindingSourceType = bindingReflectionFactory.CreateBindingSourceType(codeClass, filteredAttributes);

            if (!bindingSourceProcessor.ProcessType(bindingSourceType))
                return;

            foreach (var codeFunction in codeClass.Children.OfType<CodeFunction>())
            {
                bindingSourceProcessor.ProcessMethod(CreateBindingSourceMethod(codeFunction, bindingSourceType, bindingSourceProcessor));
            }

            bindingSourceProcessor.ProcessTypeDone();
        }

        private BindingSourceMethod CreateBindingSourceMethod(CodeFunction codeFunction, BindingSourceType bindingSourceType, IdeBindingSourceProcessor bindingSourceProcessor)
        {
            var filteredAttributes = codeFunction.Attributes.Cast<CodeAttribute2>().Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.FullName)).ToArray();
            return bindingReflectionFactory.CreateBindingSourceMethod(codeFunction, bindingSourceType, filteredAttributes);
        }

        static internal bool IsPotentialBindingClass(CodeClass codeClass)
        {
            try
            {
                var filteredAttributes = codeClass.Attributes.OfType<CodeAttribute2>();
                return BindingSourceProcessor.IsPotentialBindingClass(filteredAttributes.Select(attr => attr.FullName));
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
