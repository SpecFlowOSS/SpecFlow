using System.IO;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public abstract class FileInput
    {
        public string FileName { get; private set; }
        public string Folder { get; private set; }
        public virtual string Content { get; private set; }

        public string ProjectRelativePath
        {
            get { return Folder == "." ? FileName : Path.Combine(Folder, FileName); }
        }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(FileName); }
        }

        protected FileInput(string fileName, string folder, string content = null)
        {
            FileName = fileName;
            Folder = folder;
            Content = content;
        }
    }
}