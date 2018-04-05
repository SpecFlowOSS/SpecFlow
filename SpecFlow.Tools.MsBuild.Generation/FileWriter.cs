using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FileWriter : IWriter
    {
        public FileWriter(string filePath, TaskLoggingHelper log)
        {
            // TODO: add null check and check for valid file system path
            FilePath = filePath;
            Log = log;
            DirectoryPath = Path.GetDirectoryName(FilePath);
        }

        public string FilePath { get; }

        public TaskLoggingHelper Log { get; }

        public string DirectoryPath { get; }

        public string WriteIfNotEqual(string content)
        {
            Log.LogWithNameTag(
                Log.LogMessage,
                $"Writing data to {FilePath}; path = {DirectoryPath}; generatedFilename = {Path.GetFileName(FilePath)}");

            if (File.Exists(FilePath))
            {
                if (!FileSystemHelper.FileCompareContent(FilePath, content))
                {
                    File.WriteAllText(FilePath, content);
                }
            }
            else
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                File.WriteAllText(FilePath, content);
            }

            return FilePath;
        }
    }
}
