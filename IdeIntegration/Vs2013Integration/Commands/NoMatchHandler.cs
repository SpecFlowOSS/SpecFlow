using System;
using System.Globalization;
using System.Windows.Forms;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    class NoMatchHandler
    {
        private readonly GherkinEditorContext editorContext;
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;

        public NoMatchHandler(GherkinEditorContext editorContext, IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider)
        {
            this.editorContext = editorContext;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
        }

        internal void HandleNoMatch(GherkinStep step, CultureInfo bindingCulture)
        {
            var scope = editorContext.ProjectScope as VsProjectScope;
            var language = scope != null ? VsProjectScope.GetTargetLanguage(scope.Project) : ProgrammingLanguage.CSharp;
            var stepDefinitionSkeletonStyle = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.StepDefinitionSkeletonStyle;
            var skeleton = stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(language, step, stepDefinitionSkeletonStyle, bindingCulture);

            var result = MessageBox.Show("No matching step binding found for this step! Do you want to copy the step binding skeleton to the clipboard?"
                 + Environment.NewLine + Environment.NewLine + skeleton, "Go to binding", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                Clipboard.SetText(skeleton);
            }
        }

    }
}
