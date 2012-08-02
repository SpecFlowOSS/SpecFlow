using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.UI;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class GenerateStepDefinitionSkeletonCommand  : SpecFlowProjectSingleSelectionCommand
    {
        private readonly IProjectScopeFactory projectScopeFactory;
        private readonly IGherkinLanguageServiceFactory gherkinLanguageServiceFactory;
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;

        public GenerateStepDefinitionSkeletonCommand(IServiceProvider serviceProvider, DTE dte, IGherkinLanguageServiceFactory gherkinLanguageServiceFactory, IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider, IProjectScopeFactory projectScopeFactory) : base(serviceProvider, dte)
        {
            this.gherkinLanguageServiceFactory = gherkinLanguageServiceFactory;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
            this.projectScopeFactory = projectScopeFactory;
        }

        protected override void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            base.BeforeQueryStatus(command, selection);

            if (command.Visible)
            {
                if (dte.ActiveDocument == null || dte.ActiveDocument.ProjectItem == null || !ContextDependentNavigationCommand.IsFeatureFile(dte.ActiveDocument.ProjectItem))
                    command.Visible = false;
            }
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var activeDocument = dte.ActiveDocument;
            Invoke(activeDocument);
        }

        private void Invoke(Document activeDocument)
        {
            var editorContext = GherkinEditorContext.FromDocument(activeDocument, gherkinLanguageServiceFactory);
            if (editorContext == null) 
                return;

            GenerateStepDefinitionSkeleton(editorContext);
        }

        private bool GenerateStepDefinitionSkeleton(GherkinEditorContext editorContext)
        {
            var bindingMatchService = GetBindingMatchService(editorContext.LanguageService);
            if (bindingMatchService == null)
                return false;

            var fileScope = editorContext.LanguageService.GetFileScope(waitForLatest: true);
            if (fileScope == null)
                return false;

            var featureTitle = GetFeatureTitle(fileScope);
            var bindingCulture = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.BindingCulture ?? fileScope.GherkinDialect.CultureInfo;
            var steps = GetUnboundSteps(bindingMatchService, fileScope, bindingCulture).ToArray();

            if (steps.Length == 0)
            {
                MessageBox.Show("All steps are bound!", "Generate step definition skeleton");
                return true;
            }

            var specFlowProject = ((VsProjectScope) editorContext.ProjectScope).Project;
            var defaultLanguage = VsProjectScope.GetTargetLanguage(specFlowProject);
            var stepDefinitionSkeletonStyle = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.StepDefinitionSkeletonStyle;

            using (var skeletonGeneratorForm = new GenerateStepDefinitionSkeletonForm(featureTitle, steps, specFlowProject, stepDefinitionSkeletonStyle, defaultLanguage, dte))
            {
                skeletonGeneratorForm.OnPreview = (form) =>
                    {
                        var skeleton = stepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                            defaultLanguage, form.SelectedSteps, "MyNamespace",
                            form.ClassName, form.Style, bindingCulture);
                        MessageBox.Show(skeleton, "Step definition skeleton preview");
                    };
                skeletonGeneratorForm.OnCopy = (form) =>
                    {
                        var skeleton = string.Join(Environment.NewLine, 
                            form.SelectedSteps.Select(si => stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(
                            defaultLanguage, si, form.Style, bindingCulture)).Distinct()).Indent(StepDefinitionSkeletonProvider.METHOD_INDENT);

                        Clipboard.SetText(skeleton);
                        return true;
                    };
                skeletonGeneratorForm.OnGenerate = (form, targetPath) =>
                {
                    var project = GetTartgetProject(targetPath, specFlowProject);
                    var language = VsProjectScope.GetTargetLanguage(project);

                    var skeleton = stepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                        language, form.SelectedSteps, CalculateNamespace(targetPath, project), 
                        form.ClassName, form.Style, bindingCulture);

                    string folder = Path.GetDirectoryName(targetPath);
                    if (folder != null && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    File.WriteAllText(targetPath, skeleton, Encoding.UTF8);

                    var projectItem = VsxHelper.FindProjectItemByFilePath(project, targetPath);
                    if (projectItem == null)
                        projectItem = project.ProjectItems.AddFromFile(targetPath);

                    if (projectItem != null)
                        projectItem.Open();

                    return true;
                };

                skeletonGeneratorForm.ShowDialog();
            }

            return true;
        }

        private string CalculateNamespace(string targetPath, Project project)
        {
            string folder = Path.GetDirectoryName(targetPath);
            var rootNamespace = (string) project.Properties.Item("DefaultNamespace").Value;

            string projectRelativeFolder = FileSystemHelper.GetRelativePath(folder, VsxHelper.GetProjectFolder(project));
            if (projectRelativeFolder == ".")
            {
                return rootNamespace;
            }

            var folderItem = VsxHelper.FindFolderProjectItemByFilePath(project, folder);
            if (folderItem != null)
            {
                return (string)folderItem.Properties.Item("DefaultNamespace").Value;
            }

            return rootNamespace + "." + projectRelativeFolder.Replace("\\", ".");
        }

        private Project GetTartgetProject(string targetPath, Project specFlowProject)
        {
            string folder = Path.GetDirectoryName(targetPath);
            if (folder == null)
                return specFlowProject;

            var ps = (VsProjectScope)projectScopeFactory.GetProjectScope(specFlowProject);
            var targetProject = ps.BindingFilesTracker.BindingAssemblies.Where(ba => ba.IsProject).Select(ba => ba.Project)
                .FirstOrDefault(prj =>
                                    {
                                        var projectFolder = VsxHelper.GetProjectFolder(prj);
                                        return projectFolder != null && folder.StartsWith(projectFolder);
                                    });
            return targetProject ?? specFlowProject;
        }

        private string GetFeatureTitle(IGherkinFileScope fileScope)
        {
            if (fileScope.HeaderBlock == null || string.IsNullOrWhiteSpace(fileScope.HeaderBlock.Title))
                return "Unknown";

            return fileScope.HeaderBlock.Title;
        }

        private IEnumerable<StepInstance> GetUnboundSteps(IStepDefinitionMatchService bindingMatchService, IGherkinFileScope fileScope, CultureInfo bindingCulture)
        {
            return fileScope.GetAllStepsWithFirstExampleSubstituted().Where(s => IsListed(s, bindingMatchService, bindingCulture));
        }

        private static bool IsListed(GherkinStep step, IStepDefinitionMatchService bindingMatchService, CultureInfo bindingCulture)
        {
            List<BindingMatch> candidatingMatches;
            StepDefinitionAmbiguityReason ambiguityReason;
            var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);
            bool isListed = !match.Success;
            if (isListed && candidatingMatches.Count > 0)
            {
                isListed = ambiguityReason == StepDefinitionAmbiguityReason.AmbiguousScopes; // if it is already ambiguous (except scopes), we rather not list it
            }
            return isListed;
        }

        private IStepDefinitionMatchService GetBindingMatchService(GherkinLanguageService languageService)
        {
            var bindingMatchService = languageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            if (!bindingMatchService.Ready)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Generate step definition skeleton");
                return null;
            }

            return bindingMatchService;
        }
    }
}
