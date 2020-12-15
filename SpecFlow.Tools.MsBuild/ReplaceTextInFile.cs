using System;
using System.IO;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild
{
    public class ReplaceTextInFile : Task
    {
        [Required] 
        public string InputFile { get; set; }
        [Required] 
        public string OutputFile { get; set; }
        [Required] 
        public string TextToReplace { get; set; }
        [Required] 
        public string TextToReplaceWith { get; set; }
        public bool WriteOnlyWhenChanged { get; set; }

        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        public override bool Execute()
        {
            _readWriteLock.EnterReadLock();
            _readWriteLock.EnterWriteLock();
            try
            {
                LogNormalImportanceMessage("-----");
                LogNormalImportanceMessage("Executing ReplaceTextInFile!");
                LogNormalImportanceMessage($"WriteOnlyWhenChanged: {WriteOnlyWhenChanged}");

                var fileContent = File.ReadAllText(InputFile);
                var replacedContent = fileContent.Replace(TextToReplace, TextToReplaceWith);
                if (WriteOnlyWhenChanged && File.Exists(OutputFile))
                {
                    var existingContent = File.ReadAllText(OutputFile);
                    if (replacedContent.Equals(existingContent))
                    {
                        LogNormalImportanceMessage($"Skipping unchanged file {OutputFile}");
                        LogNormalImportanceMessage("-----");
                        return true;
                    }
                }

                var dir = Path.GetDirectoryName(OutputFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(OutputFile, replacedContent);

                LogNormalImportanceMessage("Finished ReplaceTextInFile!");
                LogNormalImportanceMessage("-----");
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
                _readWriteLock.ExitReadLock();
            }
        }

        private void LogMessage(string message, MessageImportance importance)
        {
            Log.LogMessage(importance, message);
        }

        private void LogHighImportanceMessage(string message)
        {
            LogMessage(message, MessageImportance.High);
        }

        private void LogNormalImportanceMessage(string message)
        {
            LogMessage(message, MessageImportance.Normal);
        }
    }
}
