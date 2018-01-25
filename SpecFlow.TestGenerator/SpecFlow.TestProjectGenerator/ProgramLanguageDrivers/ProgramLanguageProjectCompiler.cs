using System.Collections.Generic;
using Microsoft.Build.Evaluation;
using SpecFlow.TestProjectGenerator.Inputs;

namespace SpecFlow.TestProjectGenerator.ProgramLanguageDrivers
{
    internal abstract class ProgramLanguageProjectCompiler : IProgramLanguageProjectCompiler
    {
        protected ProjectCompilerHelper ProjectCompilerHelper;

        protected ProgramLanguageProjectCompiler(ProjectCompilerHelper projectCompilerHelper)
        {
            ProjectCompilerHelper = projectCompilerHelper;
        }
        

        public void AddBindingClass(InputProjectDriver inputProjectDriver, Project project, BindingClassInput bindingClassInput)
        {
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, BindingClassFileName, bindingClassInput.FileName, new Dictionary<string, string>
            {
                { "ClassName", bindingClassInput.Name},
                { "Bindings", GetBindingsCode(bindingClassInput)},
            });
            project.AddItem("Compile", bindingClassInput.ProjectRelativePath);
        }

        protected abstract string BindingClassFileName { get; }
        
        protected abstract string GetBindingsCode(BindingClassInput bindingClassInput);
        public abstract string FileEnding { get; }
        public abstract string ProjectFileName { get; }
        public abstract void AdditionalAdjustments(Project project, InputProjectDriver inputProjectDriver);
    }
}