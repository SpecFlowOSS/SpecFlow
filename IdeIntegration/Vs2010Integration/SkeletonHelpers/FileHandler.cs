using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    public class FileHandler : IFileHandler
    {

        public string WriteToFile(string text, bool writeOverExistingFile, string stepDefFileName)
        {
            if (!writeOverExistingFile)
            {
                if (File.Exists(stepDefFileName))
                {
                    throw new FileHandlerException(
                        "This file already exists, are you sure you wish to overwrite the file?");
                }
            }
            File.WriteAllText(stepDefFileName, text);
            return stepDefFileName;
        }

        public string WriteToFile(string text, bool writeOverExistingFile, string suggestedStepDefName, string featureFile)
        {
            string newFileName = suggestedStepDefName + ".cs";
            //set the location for the new file to be the same folder that the feature file is in.
            string fileName = Path.Combine(Path.GetDirectoryName(featureFile), newFileName);
            return WriteToFile(text, writeOverExistingFile, fileName);
        }

        /// <summary>
        /// Adds a class file to the relevant visual studio solution
        /// </summary>
        /// <param name="sln">The solution to add the file to</param>
        /// <param name="featureFile">The feature file path that the class file is for.</param>
        /// <param name="classFile">The class file to be added</param>
        /// <returns>true if the class was added succesfully</returns>
        public bool AddToSolution(Solution sln, string featureFile, string classFile)
        {
            //Locate feature file in solution
            ProjectItem prjItem = sln.FindProjectItem(featureFile);
            if (prjItem == null)
                return false;

            //Add the class file into the same project that conatins the feature file.
            prjItem.ContainingProject.ProjectItems.AddFromFile(classFile);
            return true;
        }

        public string GetFileText(string fileName, string extension)
        {
            if (fileName.EndsWith(extension))
            {
                return File.ReadAllText(fileName);
            }
            throw new FileHandlerException("The file you selected is not a class file");
        }
    }
}
