using System.IO;

namespace SpecFlow.TestProjectGenerator.Inputs
{
    public abstract class FileInput
    {
        protected FileInput(string fileName, string folder)
        {
            FileName = fileName;
            Folder = folder;
        }

        public string FileName { get; }
        public string Folder { get; }

        public string ProjectRelativePath => Folder == "." ? FileName : Path.Combine(Folder, FileName);

        public string Name => Path.GetFileNameWithoutExtension(FileName);
    }
}