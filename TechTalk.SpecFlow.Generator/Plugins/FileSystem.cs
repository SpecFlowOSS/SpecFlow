using System;
using System.IO;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IFileSystem
    {
        bool FileExists(string path);

        string[] GetDirectories(string path, string searchPattern);

        string[] GetDirectories(string path, string searchPattern, SearchOption option);
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

        public String[] GetDirectories(String path, String searchPattern, SearchOption option)
        {
            return Directory.GetDirectories(path, searchPattern, option);
        }
    }
}
