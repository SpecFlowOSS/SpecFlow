using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Tools.MsBuild
{
    public abstract class GeneratorTaskBase : TaskBase
    {
        public bool BuildServerMode { get; set; }
        public bool OverwriteReadOnlyFiles { get; set; }
        public bool OnlyUpdateIfChanged { get; set; }

        public class OutputFile
        {
            public string FilePath { get; private set; }

            public virtual string FilePathForWriting
            {
                get { return FilePath; }
            }

            public OutputFile(string filePath)
            {
                FilePath = filePath;
            }

            public virtual void Done(CompilerErrorCollection result)
            {
                // nop;
            }

            public virtual void Skip()
            {
                // nop;
            }
        }

        public abstract class TempOutputFile : OutputFile
        {
            public string TempFilePath { get; private set; }

            public override string FilePathForWriting
            {
                get { return TempFilePath; }
            }

            protected TempOutputFile(string filePath)
                : base(filePath)
            {
                TempFilePath = Path.Combine(Path.GetTempPath(), "tmp" + Path.GetFileName(filePath));
            }

            public override void Done(CompilerErrorCollection result)
            {
                SafeDeleteFile(TempFilePath);
            }

            public override void Skip()
            {
                SafeDeleteFile(TempFilePath);
            }
        }

        public class VerifyDifferenceOutputFile : TempOutputFile
        {
            public VerifyDifferenceOutputFile(string filePath) : base(filePath)
            {
            }

            public override void Done(CompilerErrorCollection result)
            {
                if (!FileSystemHelper.FileCompare(TempFilePath, FilePath))
                {
                    string message = String.Format("Error during file generation. The target file '{0}' is read-only, but different from the transformation result. This problem can be a sign of an inconsistent source code package. Compile and check-in the current version of the file from the development environment or remove the read-only flag from the generation result. To compile a solution that contains messaging project on a build server, you can also exclude the messaging project from the build-server solution or set the <OverwriteReadOnlyFiles> msbuild project parameter to 'true' in the messaging project file.", 
                        Path.GetFullPath(FilePath));
                    result.Add(new CompilerError(String.Empty, 0, 0, null, message));
                }

                base.Done(result);
            }
        }

        public class UpdateIfChangedOutputFile : TempOutputFile
        {
            public UpdateIfChangedOutputFile(string filePath) : base(filePath)
            {
            }

            public override void Done(CompilerErrorCollection result)
            {
                if (!FileSystemHelper.FileCompare(TempFilePath, FilePath))
                {
                    ReplaceFile(TempFilePath, FilePath);
                }

                base.Done(result);
            }
        }

        public OutputFile PrepareOutputFile(string outputFilePath)
        {
            if (OverwriteReadOnlyFiles)
            {
                RemoveReadOnly(outputFilePath);
            }

            bool isReadOnly = IsReadOnly(outputFilePath);
            if (isReadOnly && BuildServerMode)
                return new VerifyDifferenceOutputFile(outputFilePath);

            if (OnlyUpdateIfChanged)
                return new UpdateIfChangedOutputFile(outputFilePath);

            return new OutputFile(outputFilePath);
        }

        private static bool IsReadOnly(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (!fileInfo.Exists)
                    return false;
                return fileInfo.IsReadOnly;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "IsReadOnly");

                // if there is an exception, we let the generation to discover the real problem
                return false;
            }
        }

        private static void RemoveReadOnly(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists && fileInfo.IsReadOnly)
                fileInfo.IsReadOnly = false;
        }

        private static void SafeDeleteFile(string path)
        {
            try
            {
                if (IsReadOnly(path))
                    RemoveReadOnly(path);
                File.Delete(path);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "SaveDeleteFile");
            }
        }

        private static void ReplaceFile(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
                SafeDeleteFile(targetPath);

            File.Move(sourcePath, targetPath);
        }
    }
}