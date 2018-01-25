using Microsoft.Build.Evaluation;
using SpecFlow.TestProjectGenerator.Inputs;

namespace SpecFlow.TestProjectGenerator.ProgramLanguageDrivers
{
    public interface IProgramLanguageProjectCompiler
    {
        void AddBindingClass(InputProjectDriver inputProjectDriver, Project project, BindingClassInput bindingClassInput);
        string FileEnding { get; }
        string ProjectFileName { get; }
        void AdditionalAdjustments(Project project, InputProjectDriver inputProjectDriver);
    }
}