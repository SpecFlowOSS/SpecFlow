using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using System.Windows.Forms;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    class NoMatchHandler
    {
        private EditorCommands.GherkinEditorContext editorContext;
        private BindingSkeletons.IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;

        public NoMatchHandler(EditorCommands.GherkinEditorContext editorContext, BindingSkeletons.IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider)
        {
            this.editorContext = editorContext;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
        }

        internal void HandleNoMatch(LanguageService.GherkinStep step, System.Globalization.CultureInfo bindingCulture)
        {
            var language = editorContext.ProjectScope is VsProjectScope ? VsProjectScope.GetTargetLanguage(((VsProjectScope)editorContext.ProjectScope).Project) : ProgrammingLanguage.CSharp;
            var stepDefinitionSkeletonStyle = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.StepDefinitionSkeletonStyle;
            string skeleton = stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(language, step, stepDefinitionSkeletonStyle, bindingCulture);

            var result = MessageBox.Show("No matching step binding found for this step! Do you want to copy the step binding skeleton to the clipboard?"
                 + Environment.NewLine + Environment.NewLine + skeleton, "Go to binding", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                Clipboard.SetText(skeleton);
            }
        }

    }
}
