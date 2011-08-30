using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons;
using StepDefSkeletonInfo = TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons.StepDefSkeletonInfo;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    public class StepFileGenerator
    {
        private const string MessageBoxHeader = "Generate SpecFlow Step Definition File";
        private readonly IFileHandler _handler;
        private BindingsAnalyser _analyser;
        private Solution _sln;
        private string _featurePath, _defaultNamespace;
        private uint _projectItemId;
        private string _suggestedStepDefName;

        public StepFileGenerator(IFileHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Before the context menu is brought up this event handler is called to make the command visible for .feature files
        /// </summary>
        public void QueryStatusMenuCommandBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand; //The menu item command we are dealing with
            if (menuCommand != null)
            {
                //Get the hierarchy of files, and set _projectItemId to be the selected file
                var hierarchy = GetHierarchy();
                if (hierarchy != null)
                {
                    //Get the name of the selected file
                    hierarchy.GetCanonicalName(_projectItemId, out _featurePath);
                    //If the file ends in .feature make the menu command visible
                    if (_featurePath != null && _featurePath.EndsWith(".feature"))
                        menuCommand.Visible = true;
                    else
                        menuCommand.Visible = false;
                }
            }
        }

        /// <summary>
        /// When the menu item command to generate a step definition file is selected,
        /// this starts the step file generation and catches any special exceptions,
        /// presenting the message from that error in a message box
        /// </summary>
        public void GenerateStepFileMenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                GenerateStepFile();
            }
            catch (FileGeneratorException exception)
            {
                MessageBox.Show(exception.Message, MessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Links together the different generation parts to produce a new file or update an existing one.
        /// </summary>
        private void GenerateStepFile()
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(SDTE)); //The DTE2 provides access to the solution
            _sln = dte.Solution;

            _defaultNamespace = GetCurrentNamespaceFromSolutionExplorer();
            string projectName = GetProjectFile(); //Get the full path location of the project

            //If the project has not been built since it was last modified then the correct bindings will not have
            // established so an exception must be thrown.
            EnsureProjectBuilt();

            GenerateProject(projectName); //Builds a project and sets the bindings analyser
            
            IStepDefinitionSkeletonProvider skeletonProvider;
            string classExtension;
            ProgrammingLanguage pl = GetProgrammingLanguage(projectName);
            //Get the skeleton corresponding to the correct programming langauge
            switch (pl)
            {
                case ProgrammingLanguage.VB:
                    skeletonProvider = new StepDefinitionSkeletonProviderVB();
                    classExtension = ".vb";
                    break;
                default:
                    skeletonProvider = new StepDefinitionSkeletonProviderCS();
                    classExtension = ".cs";
                    break;
            }
            ProcessMissingSteps(skeletonProvider, classExtension);
        }

        /// <summary>
        /// Navigates the hierarchy of the selected file in the solution explorer to gather the defualt namespace.
        /// </summary>
        private string GetCurrentNamespaceFromSolutionExplorer()
        {
            var hierarchy = GetHierarchy();

            if (hierarchy == null)
            {
                throw new FileGeneratorException("No selected file can be detected");
            }
            //From the hierarchy get the namespace and the path to the feature file
            object namespaceObj;
            hierarchy.GetProperty(_projectItemId, (int)__VSHPROPID.VSHPROPID_DefaultNamespace, out namespaceObj);
            if(namespaceObj == null)
                throw new FileGeneratorException("The namespace could not be located, ensure the feature file is located in a SpecFLow Project.");
            return namespaceObj.ToString();
        }

        /// <summary>
        /// Builds a specFlow project, parses a feature corresponding to the _featurePath and analyses the projects bindings.
        /// </summary>
        /// <param name="projectName">The full path of the project to generate</param>
        private void GenerateProject(string projectName)
        {

            //Build project
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectName);
            //Prase feature
            Feature feature = ParseFeature(_featurePath, specFlowProject);
            if (feature == null)
            {
                throw new FileGeneratorException("Could not find a SpecFlow project containing your feature file");
            }
            _suggestedStepDefName = "StepsFor" + feature.Title.ToIdentifier();

            //The bin folder to collect the bindings from must be calculated.
            Project vsProject = GetProject();
            string binFolder = vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

            _analyser = new BindingsAnalyser(specFlowProject, feature, binFolder); //Collect bindings
            if (_analyser.Bindings == null)
            {
                throw new FileGeneratorException("Bindings are still being analysed.. Please Wait");
            }
        }

        /// <summary>
        /// Generates the missing steps for the given feature file and determines whether they
        /// should be added to an existing file or put in a new file
        /// </summary>
        private void ProcessMissingSteps(IStepDefinitionSkeletonProvider skeletonProvider, string classExtension)
        {
            var missingSteps = _analyser.GetMissingSteps(); //Get the steps which did not match any bindings
            if (missingSteps.Count <= 0)
            {
                throw new FileGeneratorException("There are no unimplemented steps for this feature.");
            }

            var result = MessageBox.Show("Do you wish to add your step definitions to an existing file?",
                                         MessageBoxHeader, MessageBoxButtons.YesNoCancel,
                                         MessageBoxIcon.Question);
            switch (result)
            {
                case DialogResult.No:
                    {
                        WriteStepsToFile(skeletonProvider, missingSteps);
                        break;
                    }
                case DialogResult.Yes:
                    {
                        AddStepsToFile(skeletonProvider, missingSteps, classExtension);
                        break;
                    }
            }
        }

        /// <summary>
        /// Creates a new file containing the missing steps and adds them to the visual studio
        /// solution. Then displays a message conveying whether it succeeded.
        /// </summary>
        private void WriteStepsToFile(IStepDefinitionSkeletonProvider skeletonProvider, List<StepInstance> missingSteps)
        {
            //Store info on file
            StepDefSkeletonInfo info = new StepDefSkeletonInfo(_suggestedStepDefName,
                                                               _defaultNamespace);
            //Generate the skeleton for the new file
            string skeleton = skeletonProvider.GetFileSkeleton(missingSteps, info);
            string file;
            try
            {
                //Try to write the skeleton to a file
                file = _handler.WriteToFile(skeleton, false, _suggestedStepDefName, _featurePath);
            }
            catch (FileHandlerException fileHandlerException)
            {
                //If the file already existed, ask to overwrite
                var overwrite = MessageBox.Show(fileHandlerException.Message, MessageBoxHeader,
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //If they have selected to overwrite then add again, this time allowing overwrites
                if (overwrite == DialogResult.Yes)
                    file = _handler.WriteToFile(skeleton, true, _suggestedStepDefName, _featurePath);
                else
                    throw new FileGeneratorException("Cancelled creating step definition file.");
            }
            //Add the file generated to the visual studio solution
            if (!_handler.AddToSolution(_sln, _featurePath, file))
                MessageBox.Show(
                    "A step defintion file has been created but it could not be added to an existing visual studio project",
                    MessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Success! A step definition file has been generated",
                                MessageBoxHeader);
        }

        /// <summary>
        /// Adds the missing steps to a file already existing through use of the skeletonProvider.
        /// Shows a message if succesful.
        /// </summary>
        private void AddStepsToFile(IStepDefinitionSkeletonProvider skeletonProvider,
            List<StepInstance> missingSteps, string classExtension)
        {
            string dir = Path.GetDirectoryName(_featurePath);
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = ".cs",
                Title = "Choose the file your step definitions should be added to.",
                InitialDirectory = dir,
                FileName = _suggestedStepDefName
            };
            DialogResult dialogResult = ofd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    string contents = _handler.GetFileText(ofd.FileName, classExtension);
                    string newText = skeletonProvider.AddStepsToExistingFile(contents, missingSteps);
                    if (!String.IsNullOrEmpty(newText))
                    {
                        _handler.WriteToFile(newText, true, ofd.FileName);
                        MessageBox.Show("Success! Your steps have been added successfully",
                                        MessageBoxHeader);
                    }
                    else
                    {
                        MessageBox.Show("The file you selected does not contain a binding class",
                                        MessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (FileHandlerException fileHandlerException)
                {
                    MessageBox.Show(fileHandlerException.Message, MessageBoxHeader,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Retrieves the hierarchy containing the currently selected file in the visual studio
        /// solution explorer and sets _ProjectItemId to be the value corresponding to the currently
        /// selected file.
        /// </summary>
        private IVsHierarchy GetHierarchy()
        {
            IntPtr hierarchyPtr, selectionContainerPtr;
            IVsMultiItemSelect mis;

            //Get the currently selected file in the solution explorer
            IVsMonitorSelection monitorSelection =
                (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            monitorSelection.GetCurrentSelection(out hierarchyPtr, out _projectItemId, out mis, out selectionContainerPtr);

            //Get the hierarchy containing the file
            return Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
        }

        /// <summary>
        /// Searches for the visual studio solution for the feature file selected,
        /// it then calculates the path to the project containing that feature file.
        /// </summary>
        private string GetProjectFile()
        {
            Project prj = GetProject();
            if (String.IsNullOrEmpty(prj.FileName))
            {
                throw new FileGeneratorException("Could not find a SpecFlow project containing your feature file");
            }
            return prj.FileName;
        }
        private Project GetProject()
        {
            ProjectItem prjItem = _sln.FindProjectItem(_featurePath);
            if (prjItem == null || prjItem.ContainingProject == null)
            {
                throw new FileGeneratorException("Could not find a SpecFlow project containing your feature file");
            }
            return prjItem.ContainingProject;
        }

        /// <summary>
        /// If the project is dirty this will throw an exception.
        /// </summary>
        private void EnsureProjectBuilt()
        {
            Project project = _sln.FindProjectItem(_featurePath).ContainingProject;
            _sln.SolutionBuild.BuildProject(project.ConfigurationManager.ActiveConfiguration.ConfigurationName, project.UniqueName, true);
        }

        /// <summary>
        /// Loops through the feature files in a project and returns the feature object corresponding
        /// to the targetFeature path
        private static Feature ParseFeature(string targetFeature, SpecFlowProject specFlowProject)
        {
            SpecFlowLangParser parser = new SpecFlowLangParser(specFlowProject.Configuration.GeneratorConfiguration.FeatureLanguage);
            using (var reader = new StreamReader(targetFeature))
            {
                return parser.Parse(reader, targetFeature);
            }
        }

        private static ProgrammingLanguage GetProgrammingLanguage(string projectName)
        {
            if (projectName.EndsWith(".csproj"))
                return ProgrammingLanguage.CSharp;
            if (projectName.EndsWith(".vbproj"))
                return ProgrammingLanguage.VB;
            return ProgrammingLanguage.Other;
        }
    }
}
