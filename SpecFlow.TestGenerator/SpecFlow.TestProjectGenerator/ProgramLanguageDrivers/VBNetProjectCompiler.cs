using System.IO;
using System.Text;
using Microsoft.Build.Evaluation;
using SpecFlow.TestProjectGenerator.Inputs;

namespace SpecFlow.TestProjectGenerator.ProgramLanguageDrivers
{
    internal class VBNetProjectCompiler : ProgramLanguageProjectCompiler
    {
        public VBNetProjectCompiler(ProjectCompilerHelper projectCompilerHelper) : base(projectCompilerHelper)
        {
        }

        protected override string BindingClassFileName => "BindingClass.vb";
        public override string FileEnding => ".vb";

        public override string ProjectFileName => "TestProjectFile.vbproj";

        protected override string GetBindingsCode(BindingClassInput bindingClassInput)
        {
            var result = new StringBuilder();

            var counter = 0;

            foreach (var stepBindingInput in bindingClassInput.StepBindings)
            {
                result.AppendFormat(@"<[{2}](""{3}"")> Public Sub sb{0}() 
                                        {1}
                                      End Sub", ++counter, stepBindingInput.VBNetCode, stepBindingInput.ScenarioBlock, stepBindingInput.Regex);
                result.AppendLine();
            }

            foreach (var otherBinding in bindingClassInput.OtherBindings)
            {
                result.AppendLine(otherBinding);
            }

            return result.ToString();
        }

        public override void AdditionalAdjustments(Project project, InputProjectDriver inputProjectDriver)
        {
            var myProjectFolder = Path.Combine(inputProjectDriver.ProjectFolder, "My Project");
            Directory.CreateDirectory(myProjectFolder);

            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.Application.Designer.vb", @"My Project\Application.Designer.vb");
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.Application.myapp", @"My Project\Application.myapp");
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.AssemblyInfo.vb", @"My Project\AssemblyInfo.vb");
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.Resources.Designer.vb", @"My Project\Resources.Designer.vb");
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.Resources.resx.content", @"My Project\Resources.resx");
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.Settings.Designer.vb", @"My Project\Settings.Designer.vb");
            ProjectCompilerHelper.SaveFileFromResourceTemplate(inputProjectDriver.ProjectFolder, @"My_Project.Settings.settings", @"My Project\Settings.settings");
        }
    }
}