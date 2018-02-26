using System.Text;
using Microsoft.Build.Evaluation;
using SpecFlow.TestProjectGenerator.Inputs;

namespace SpecFlow.TestProjectGenerator.ProgramLanguageDrivers
{
    class CSharpProgramLanguageProjectCompiler : ProgramLanguageProjectCompiler
    {

        public CSharpProgramLanguageProjectCompiler(ProjectCompilerHelper projectCompilerHelper) : base(projectCompilerHelper)
        {

        }

        public override string FileEnding => ".cs";
        public override string ProjectFileName
        {
            get
            {
                if (CurrentVersionDriver.SpecFlowMajor < 2)
                {
                    return "TestProjectFile_Before20.csproj";
                }
                return "TestProjectFile.csproj";
            }
        }

        public override void AdditionalAdjustments(Project project, InputProjectDriver inputProjectDriver)
        {
            
        }

        protected override string BindingClassFileName => "BindingClass.cs";

        protected override string GetBindingsCode(BindingClassInput bindingClassInput)
        {
            StringBuilder result = new StringBuilder();

            int counter = 0;

            foreach (var stepBindingInput in bindingClassInput.StepBindings)
            {
                result.AppendFormat(@"[{2}(@""{3}"")]public void sb{0}() {{ 
                                        {1}
                                      }}", ++counter, stepBindingInput.CSharpCode, stepBindingInput.ScenarioBlock, stepBindingInput.Regex);
                result.AppendLine();
            }

            foreach (var otherBinding in bindingClassInput.OtherBindings)
            {
                result.AppendLine(otherBinding);
            }

            return result.ToString();
        }
    }
}