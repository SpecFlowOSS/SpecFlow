using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class VirtualFileSystem : IFileSystem
    {
        private readonly List<string> existingFiles = new List<string>();
        private bool isUnix;

        public VirtualFileSystem()
            : this(false)
        {
        }

        public VirtualFileSystem(bool isUnix)
        {
            this.isUnix = isUnix;
        }

        public IList<string> ProbedPaths { get; } = new List<string>();

        public void AddFiles(string path, params string[] paths)
        {
            this.existingFiles.Add(Path.GetFullPath(path));
            this.existingFiles.AddRange(paths.Select(Path.GetFullPath));
        }

        public bool FileExists(string path)
        {
            this.ProbedPaths.Add(path);
            return this.existingFiles.Contains(path);
        }

        public bool DirectoryExists(string path)
        {
            return this.existingFiles
                .Select(x => EnsurePathEndsWithDirectorySeparator(Path.GetDirectoryName(x)))
                .Any(x => x.StartsWith(EnsurePathEndsWithDirectorySeparator(path), StringComparison.OrdinalIgnoreCase));
        }

        public string[] GetDirectories(string path)
        {
            return this.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).ToArray();
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return this.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
        }

        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return this.EnumerateDirectories(path, searchPattern, searchOption).ToArray();
        }

        public IEnumerable<string> EnumerateDirectories(string path)
        {
            return this.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return this.EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            path = Path.GetFullPath(path);
            path = EnsurePathEndsWithDirectorySeparator(path);

            return this.GetFilesInternal(this.GetDirectories(path, this.existingFiles), path, searchPattern, searchOption);
        }

        private string[] GetDirectories(string path, List<string> existingFiles)
        {
            IEnumerable<string> Enumerate()
            {
                path = EnsurePathEndsWithDirectorySeparator(path);

                var paths = existingFiles
                    .Select(x => EnsurePathEndsWithDirectorySeparator(Path.GetDirectoryName(x)))
                    .Where(x => x.StartsWith(EnsurePathEndsWithDirectorySeparator(path), StringComparison.OrdinalIgnoreCase));

                foreach (var directoryPath in paths)
                {
                    var directory = new DirectoryInfo(directoryPath);

                    do
                    {
                        yield return directory.FullName;

                        directory = directory.Parent;
                    }
                    while (directory.Parent != null);

                    yield return directory.Root.FullName;
                }
            }

            return Enumerate().Distinct().ToArray();
        }

        private string[] GetFilesInternal(IEnumerable<string> files, string path, string searchPattern, SearchOption searchOption)
        {
            if (!files.Any())
            {
                throw new DirectoryNotFoundException($"Could not find a part of the path '{path}'.");
            }

            CheckSearchPattern(searchPattern);
            path = Path.GetFullPath(path);
            path = EnsurePathEndsWithDirectorySeparator(path);

            string allDirectoriesPattern = this.isUnix
                ? @"([^<>:""/|?*]*/)*"
                : @"([^<>:""/\\|?*]*\\)*";

            string fileNamePattern;
            string pathPatternSpecial = null;
            if (searchPattern == "*")
            {
                fileNamePattern = this.isUnix ? @"[^/]*?/?" : @"[^\\]*?\\?";
            }
            else
            {
                fileNamePattern = Regex.Escape(searchPattern)
                    .Replace(@"\*", this.isUnix ? @"[^<>:""/|?*]*?" : @"[^<>:""/\\|?*]*?")
                    .Replace(@"\?", this.isUnix ? @"[^<>:""/|?*]?" : @"[^<>:""/\\|?*]?");

                var extension = Path.GetExtension(searchPattern);
                bool hasExtensionLengthOfThree = extension != null && extension.Length == 4 && !extension.Contains("*") && !extension.Contains("?");
                if (hasExtensionLengthOfThree)
                {
                    var fileNamePatternSpecial = string.Format(CultureInfo.InvariantCulture, "{0}[^.]", fileNamePattern);
                    pathPatternSpecial = string.Format(
                        CultureInfo.InvariantCulture,
                        this.isUnix ? @"(?i:^{0}{1}{2}(?:/?)$)" : @"(?i:^{0}{1}{2}(?:\\?)$)",
                        Regex.Escape(path),
                        searchOption == SearchOption.AllDirectories ? allDirectoriesPattern : string.Empty,
                        fileNamePatternSpecial);
                }
            }

            var pathPattern = string.Format(
                CultureInfo.InvariantCulture,
                this.isUnix ? @"(?i:^{0}{1}{2}(?:/?)$)" : @"(?i:^{0}{1}{2}(?:\\?)$)",
                Regex.Escape(path),
                searchOption == SearchOption.AllDirectories ? allDirectoriesPattern : string.Empty,
                fileNamePattern);


            return files
                .Where(p =>
                {
                    if (Regex.IsMatch(p, pathPattern))
                    {
                        return true;
                    }

                    if (pathPatternSpecial != null && Regex.IsMatch(p, pathPatternSpecial))
                    {
                        return true;
                    }

                    return false;
                })
                .ToArray();
        }

        private static string EnsurePathEndsWithDirectorySeparator(string path)
        {
            if (!path.EndsWith(string.Format(CultureInfo.InvariantCulture, "{0}", Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
            {
                path += Path.DirectorySeparatorChar;
            }

            return path;
        }

        static void CheckSearchPattern(string searchPattern)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }

            const string TWO_DOTS = "..";
            Func<ArgumentException> createException = () => new ArgumentException(@"Search pattern cannot contain "".."" to move up directories and can be contained only internally in file/directory names, as in ""a..b"".", searchPattern);

            if (searchPattern.EndsWith(TWO_DOTS, StringComparison.OrdinalIgnoreCase))
            {
                throw createException();
            }

            int position;
            if ((position = searchPattern.IndexOf(TWO_DOTS, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                var characterAfterTwoDots = searchPattern[position + 2];
                if (characterAfterTwoDots == Path.DirectorySeparatorChar || characterAfterTwoDots == Path.AltDirectorySeparatorChar)
                {
                    throw createException();
                }
            }

            var invalidPathChars = Path.GetInvalidPathChars();
            if (searchPattern.IndexOfAny(invalidPathChars) > -1)
            {
                throw new ArgumentException("Illegal characters in path", "searchPattern");
            }
        }
    }
}
