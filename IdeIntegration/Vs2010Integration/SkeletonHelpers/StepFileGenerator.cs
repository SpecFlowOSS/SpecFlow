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
//using TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    public class StepFileGenerator
    {
        private const string MessageBoxHeader = "Generate SpecFlow Step Definition File";
        private IFileHandler _handler;
        private BindingsAnalyser _analyser;
        private Solution _sln;
        private string _featurePath, _namespaceName;
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
                var hierarchy = GetHierarchy();
                if (hierarchy != null)
                {
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
        /// this starts the step fiel generatino and catches any specflow exceptions,
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
        /// Links together the different generation parts to produce a new file, update and existing one
        /// or throw an expection if neither is suitable
        /// </summary>
        private void GenerateStepFile()
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(SDTE)); //The DTE2 provides access to the solution
            _sln = dte.Solution;

            string projectName = ProcessSelectedFile();
            GenerateFeatureInformation(projectName);
            //Get the skeleton corresponding to the correct programming langauge
            IStepDefinitionSkeletonProvider skeletonProvider;
            string classExtension;
            ProgrammingLanguage pl = GetProgrammingLanguage(projectName);
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
        /// Navigates the hierarchy of selected files to gather the basic info on the selected feature file
        /// </summary>
        private string ProcessSelectedFile()
        {
            var hierarchy = GetHierarchy();

            if (hierarchy == null)
            {
                throw new FileGeneratorException("No selected file can be detected");
            }
            //From the hierarchy get the namespace and the path to the feature file
            object namespaceObj;
            hierarchy.GetProperty(_projectItemId, (int)__VSHPROPID.VSHPROPID_DefaultNamespace, out namespaceObj);
            _namespaceName = namespaceObj.ToString();

            string projectName = GetProjectFile(); //Get the location of the project
            if (String.IsNullOrEmpty(projectName))
            {
                throw new FileGeneratorException("Could not find a SpecFlow project containing your feature file");
            }
            return projectName;
        }

        private void GenerateFeatureInformation(string projectName)
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectName); //handle expcetions
            Feature feature = ParseFeature(_featurePath, specFlowProject);
            if (feature == null)
            {
                throw new FileGeneratorException("Could not find a SpecFlow project containing your feature file");
            }
            _suggestedStepDefName = "StepsFor" + feature.Title.ToIdentifier();

            _analyser = new BindingsAnalyser(specFlowProject, feature); //Collect bindings
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

            var result = MessageBox.Show("Do you wish to add your step defintions to an existing file?",
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
            StepDefSkeletonInfo info = new StepDefSkeletonInfo(_suggestedStepDefName,
                                                               _namespaceName);
            //Gather info on file
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
                {
                    MessageBox.Show("Cancelled creating step definition file.", MessageBoxHeader);
                    return;
                }
            }
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
                        string newContents = skeletonProvider.AddStepsToExistingFile(contents, missingSteps);
                        _handler.WriteToFile(newContents, true, ofd.FileName);
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
        /// Retrieves the hierarchy containing the currently selected file in the visual studio solution explorer
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

        private string GetProjectFile()
        {
            ProjectItem prjItem = _sln.FindProjectItem(_featurePath);
            return prjItem == null ? null : prjItem.ContainingProject.FileName;
        }

        /// <summary>
        /// Loops through the feature files in a project and returns the feature object corresponding
        /// to the targetFeature path
        private static Feature ParseFeature(string targetFeature, SpecFlowProject specFlowProject)
        {
            var featureFiles =
                specFlowProject.FeatureFiles.Select(ff => ff.GetFullPath(specFlowProject.ProjectSettings));
            foreach (var featureFile in featureFiles)
            {
                SpecFlowLangParser parser = new SpecFlowLangParser(specFlowProject.Configuration.GeneratorConfiguration.FeatureLanguage);
                if (targetFeature.ToLower() == featureFile.ToLower())
                    using (var reader = new StreamReader(featureFile))
                    {
                        return parser.Parse(reader, featureFile);
                    }
            }
            return null;
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
