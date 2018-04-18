using System.Collections.Generic;
using System.IO;

namespace TechTalk.SpecFlow.Utils
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        bool DirectoryExists(string path);
        string[] GetDirectories(string path);
        string[] GetDirectories(string path, string searchPattern);
        string[] GetDirectories(string path, string searchPattern, SearchOption searchOptions);
        IEnumerable<string> EnumerateDirectories(string path);
        IEnumerable<string> EnumerateDirectories(string path, string searchPattern);
        IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOptions);
    }
}