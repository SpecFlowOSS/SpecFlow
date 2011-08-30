using EnvDTE;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    public interface IFileHandler
    {
        string WriteToFile(string text, bool writeOverExistingFile, string stepDefFileName);
        string WriteToFile(string text, bool writeOverExistingFile, string suggestedStepDefName, string featureFile);

        /// <summary>
        /// Adds a class file to the relevant visual studio solution
        /// </summary>
        /// <param name="sln">The solution to add the file to</param>
        /// <param name="featureFile">The feature file path that the class file is for.</param>
        /// <param name="classFile">The class file to be added</param>
        /// <returns>true if the class was added succesfully</returns>
        bool AddToSolution(Solution sln, string featureFile, string classFile);

        string GetFileText(string fileName, string extension);
    }
}