using System.IO;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IFileSystem
    {
        bool FileExists(string path);

        string[] GetDirectories(string path, string searchPattern);
    }

    public class FileSystem : IFileSystem
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }
    }
}
