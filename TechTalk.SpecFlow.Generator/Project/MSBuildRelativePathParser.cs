using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.Project
{
    public interface IMSBuildRelativePathParser
    {
        List<string> GetFiles(string projectFolder, string path);
    }

    class MSBuildRelativePathParser : IMSBuildRelativePathParser
    {
        public List<string> GetFiles(string projectFolder, string path)
        {
            var searchOption = path.Contains("**") ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (path.Contains("*"))
            {
                var split = path.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
                var realPath = split.Take(split.Length - 1).Aggregate((current, next) => current + next).Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}");
                var fileName = split.Last();

                return Directory.GetFiles(Path.Combine(projectFolder, realPath), "*" + fileName, searchOption)
                    .Select(i => i.Replace(projectFolder + Path.DirectorySeparatorChar, ""))
                    .ToList();
            }

            return new List<string>() { path };
        }
    }
}