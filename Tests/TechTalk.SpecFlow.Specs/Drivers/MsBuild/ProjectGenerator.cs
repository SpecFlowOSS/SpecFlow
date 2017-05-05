using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;
using TechTalk.SpecFlow.Specs.Drivers.Templates;

namespace TechTalk.SpecFlow.Specs.Drivers.MsBuild
{
    public class ProjectGenerator
    {
        private readonly TemplateManager templateManager;

        public ProjectGenerator(TemplateManager templateManager)
        {
            this.templateManager = templateManager;
        }

        private string GetProjectFileExtension(string language)
        {
            switch (language.ToLower())
            {
                case "c#":
                    return "csproj";
                case "f#":
                    return "fsproj";
                case "vb.net":
                    return "vbproj";
                default:
                    throw new NotSupportedException("Language not supported: " + language);
            }
        }

        public Project GenerateProject(InputProjectDriver inputProjectDriver)
        {
            string projectFileName = string.Format("{0}_{1}.{2}", inputProjectDriver.ProjectName, Guid.NewGuid().ToString("N"), GetProjectFileExtension(inputProjectDriver.Language));

            Console.WriteLine("Compiling project '{0}' in '{1}'", projectFileName, inputProjectDriver.CompilationFolder);

            EnsureCompilationFolder(inputProjectDriver.CompilationFolder);

            Project project = CreateProject(inputProjectDriver, projectFileName);

            AddAppConfig(inputProjectDriver, project);

            foreach (var bindingClassInput in inputProjectDriver.BindingClasses)
                AddBindingClass(inputProjectDriver, project, bindingClassInput);

            foreach (var featureFileInput in inputProjectDriver.FeatureFiles)
            {
                string outputPath = Path.Combine(inputProjectDriver.CompilationFolder, featureFileInput.ProjectRelativePath);
                File.WriteAllText(outputPath, featureFileInput.Content, Encoding.UTF8);
                var generatedFile = featureFileInput.ProjectRelativePath + "." + inputProjectDriver.CodeFileExtension;
                project.AddItem("None", featureFileInput.ProjectRelativePath, new[]
                                                                                  {
                                                                                      new KeyValuePair<string, string>("Generator", "SpecFlowSingleFileGenerator"),
                                                                                      new KeyValuePair<string, string>("LastGenOutput", generatedFile),
                                                                                  });
                project.AddItem("Compile", generatedFile);
            }

            foreach (var contentFileInput in inputProjectDriver.ContentFiles)
            {
                string outputPath = Path.Combine(inputProjectDriver.CompilationFolder, contentFileInput.ProjectRelativePath);
                File.WriteAllText(outputPath, contentFileInput.Content, Encoding.UTF8);
                project.AddItem("Content", contentFileInput.ProjectRelativePath, new[]
                                                                                  {
                                                                                      new KeyValuePair<string, string>("CopyToOutputDirectory", "PreserveNewest"),
                                                                                  });
            }


            AddReference(inputProjectDriver, project, "Newtonsoft.Json.dll");
            foreach (var reference in inputProjectDriver.References.Concat(inputProjectDriver.AppConfigConfigurationDriver.GetAdditionalReferences()))
                AddReference(inputProjectDriver, project, reference);

            project.Save();

            return project;
        }

        private void AddReference(InputProjectDriver inputProjectDriver, Project project, string reference)
        {
            string referenceFullPath = Path.GetFullPath(Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), reference));
            string assemblyName = Path.GetFileNameWithoutExtension(referenceFullPath);
            Debug.Assert(assemblyName != null);

            project.AddItem("Reference", assemblyName, new[]
                                                           {
                                                               new KeyValuePair<string, string>("HintPath", referenceFullPath),
                                                           });
        }

        private void AddBindingClass(InputProjectDriver inputProjectDriver, Project project, BindingClassInput bindingClassInput)
        {
            if (bindingClassInput.RawClass != null && bindingClassInput.RawClass.Contains("[assembly:"))
            {
                string outputPath = Path.Combine(inputProjectDriver.CompilationFolder, bindingClassInput.ProjectRelativePath);
                File.WriteAllText(outputPath, bindingClassInput.RawClass, Encoding.UTF8);
            }
            else if (bindingClassInput.RawClass != null)
            {
                SaveFileFromTemplate(inputProjectDriver.CompilationFolder, "BindingClass." + inputProjectDriver.CodeFileExtension, bindingClassInput.FileName, new Dictionary<string, string>
                                                                                    {
                                                                                        { "AdditionalUsings", "" },
                                                                                        { "BindingClass", bindingClassInput.RawClass },
                                                                                    });
            }
            else
            {
                SaveFileFromTemplate(inputProjectDriver.CompilationFolder, "Bindings." + inputProjectDriver.CodeFileExtension, bindingClassInput.FileName, new Dictionary<string, string>
                                                                                    {
                                                                                        { "ClassName", bindingClassInput.Name },
                                                                                        { "Bindings", GetBindingsCode(bindingClassInput, inputProjectDriver) },
                                                                                    });
            }
            project.AddItem("Compile", bindingClassInput.ProjectRelativePath);
        }

        private string GetBindingsCode(BindingClassInput bindingClassInput, InputProjectDriver inputProjectDriver)
        {
            StringBuilder result = new StringBuilder();

            int counter = 0;

            foreach (var stepBindingInput in bindingClassInput.StepBindings)
            {

                switch (inputProjectDriver.Language.ToLower())
                {
                    case "c#":
                        result.AppendFormat(@"[{2}(@""{3}"")]public void sb{0}({4} {5}) {{ 
                                        {1}
                                      }}", ++counter, stepBindingInput.Code, stepBindingInput.ScenarioBlock, stepBindingInput.Regex, stepBindingInput.ParameterType, stepBindingInput.ParameterName);
                        break;
                    case "vb.net":
                        if (String.IsNullOrEmpty(stepBindingInput.ParameterType))
                        {
                            result.AppendFormat(@"<[{2}](""{3}"")> Public Sub sb{0}()  
                                        {1}
                                      End Sub", ++counter, stepBindingInput.Code, stepBindingInput.ScenarioBlock, stepBindingInput.Regex);
                        }
                        else
                        {
                            result.AppendFormat(@"<[{2}](""{3}"")> Public Sub sb{0}({4} as {5})  
                                        {1}
                                      End Sub", ++counter, stepBindingInput.Code, stepBindingInput.ScenarioBlock, stepBindingInput.Regex, stepBindingInput.ParameterName, stepBindingInput.ParameterType);
                        }
                        break;
                }

                result.AppendLine();
            }

            foreach (var otherBinding in bindingClassInput.OtherBindings)
            {
                result.AppendLine(otherBinding);
            }

            return result.ToString();
        }

        private void AddAppConfig(InputProjectDriver inputProjectDriver, Project project)
        {
            inputProjectDriver.AppConfigConfigurationDriver.SaveConfigurationTo(Path.Combine(inputProjectDriver.CompilationFolder, "App.config"));
            project.AddItem("None", "App.config");

            if (inputProjectDriver.SpecFlowJsonConfigurationDriver.IsUsed)
            {
                inputProjectDriver.SpecFlowJsonConfigurationDriver.Save(Path.Combine(inputProjectDriver.CompilationFolder, "specflow.json"));
                IEnumerable<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Link", "specflow.json"),
                    new KeyValuePair<string, string>("CopyToOutputDirectory", "Always")
                };
                var projectItems = project.AddItem("None", "specflow.json", metadata );
                
            }
        }

        private string SaveFileFromTemplate(string compilationFolder, string templateName, string outputFileName, Dictionary<string, string> replacements = null)
        {
            if (replacements == null)
                replacements = new Dictionary<string, string>();

            replacements.Add("SpecFlowRoot", Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), "SpecFlow"));
            replacements.Add("LibRoot", AssemblyFolderHelper.GetTestAssemblyFolder());

            string fileContent = templateManager.LoadTemplate(templateName, replacements);

            string outputPath = Path.Combine(compilationFolder, outputFileName);
            File.WriteAllText(outputPath, fileContent, Encoding.UTF8);
            return outputPath;
        }

        private Project CreateProject(InputProjectDriver inputProjectDriver, string outputFileName)
        {
            // the MsBuild global collection caches the project file, so we need to generate a unique project file name.

            Guid projectId = Guid.NewGuid();

            string projectFileName = SaveFileFromTemplate(inputProjectDriver.CompilationFolder, "TestProjectFile." + GetProjectFileExtension(inputProjectDriver.Language), outputFileName, new Dictionary<string, string>
                                                                                    {
                                                                                        { "ProjectGuid", projectId.ToString("B") },
                                                                                        { "ProjectName", inputProjectDriver.ProjectName },
                                                                                    });

            inputProjectDriver.ProjectFilePath = projectFileName;

            ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
            return new Project(projectFileName);
        }

        private void EnsureCompilationFolder(string compilationFolder)
        {
            EnsureEmptyFolder(compilationFolder);
        }

        private static void EnsureEmptyFolder(string folderName)
        {
            folderName = Path.GetFullPath(folderName);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
                return;
            }

            foreach (string file in Directory.GetFiles(folderName))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    throw new IOException("Unable to delete file: " + file, ex);
                }
            }

            foreach (string folder in Directory.GetDirectories(folderName))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception ex)
                {
                    throw new IOException("Unable to delete folder: " + folder, ex);
                }
            }
        }
    }
}
