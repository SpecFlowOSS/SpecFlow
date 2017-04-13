using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Generator.Plugins;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class MockFileSystem : IFileSystem
    {
        private readonly String[] filesAndDirectories;

        /// <summary>
        /// Directories should end in / or \ and files should not.
        /// </summary>
        public MockFileSystem(params string[] filesAndDirectories)
        {
            this.filesAndDirectories = ExplodePaths(filesAndDirectories).ToArray();
        }

        public bool FileExists(String path)
        {
            return this.filesAndDirectories.Contains(path, StringComparer.OrdinalIgnoreCase);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public string[] GetDirectories(string path, string searchPattern, SearchOption option)
        {
            path = Path.GetFullPath(path);

            var directories = this.filesAndDirectories
                .Select(Path.GetFullPath)
                .Select(Path.GetDirectoryName)
                .Where(x => x != null)
                .Distinct()
                .ToList();

            return directories.Where(directory =>
                directory.Equals(path, StringComparison.OrdinalIgnoreCase) ||
                directory.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                directory.StartsWith(path + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                .Select(directory => new
                {
                    FullPath = directory,
                    PathParts = GetPathParts(directory
                        .Substring(path.Length, directory.Length - path.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    )
                })
                .Where(x =>
                {
                    if (option == SearchOption.AllDirectories)
                    {
                        return x.PathParts.Any(part => MatchesWildcardPattern(part, searchPattern));
                    }
                    else if (x.PathParts.Length == 1)
                    {
                        return MatchesWildcardPattern(x.PathParts[0], searchPattern);
                    }

                    return false;
                })
                .Select(x => x.FullPath)
                .ToArray();
        }

        private static string[] GetPathParts(string path)
        {
            return path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///   Checks if name matches pattern with '?' and '*' wildcards.
        /// </summary>
        private static bool MatchesWildcardPattern(string input, string pattern)
        {
            pattern = pattern.Replace("?*", "*");
            pattern = pattern.Replace("*?", "*");
            pattern = pattern.Replace("**", "*");

            if (pattern.Equals("*"))
            {
                pattern = ".*";
            }
            else
            {
                pattern = pattern.Replace(".", @"\.");
                pattern = pattern.Replace("*", @".*");
                pattern = pattern.Replace("?", @".");

                pattern = String.Concat(@"\b", pattern, @"\b");
            }

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(input);
        }

        private static IEnumerable<string> ExplodePath(string path)
        {
            var parts = GetPathParts(path).ToList();
            parts.RemoveAt(parts.Count() - 1);

            var previous = String.Empty;

            foreach (var part in parts)
            {
                yield return previous = Path.Combine(previous, part) + Path.DirectorySeparatorChar;
            }

            yield return path;
        }

        private static IEnumerable<string> ExplodePaths(IEnumerable<string> paths)
        {
            return paths.SelectMany(x => ExplodePath(x)).Distinct();
        }
    }
}
