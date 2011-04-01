using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Utils
{
    // this class is based on the class found at http://www.vcskicks.com/code-snippet/temp-file-class.php

    public class TempFile : IDisposable
    {
        private readonly string _tmpfile;

        public TempFile()
            : this(string.Empty)
        { }

        public TempFile(string extension)
        {
            _tmpfile = Path.GetTempFileName();
            if (!string.IsNullOrEmpty(extension))
            {
                string newTmpFile = _tmpfile + extension;

                // create tmp-File with new extension ...
                File.Create(newTmpFile).Dispose();
                // delete old tmp-File
                File.Delete(_tmpfile);

                // use new tmp-File
                _tmpfile = newTmpFile;
            }
        }

        public void SetContent(string fileContent)
        {
            using (StreamWriter writer = new StreamWriter(FullPath, false, Encoding.UTF8))
            {
                writer.Write(fileContent);
            }
        }

        public string FullPath
        {
            get { return _tmpfile; }
        }

        public string FileName
        {
            get { return Path.GetFileName(FullPath); }
        }

        public string FolderName
        {
            get { return Path.GetDirectoryName(FullPath); }
        }

        void IDisposable.Dispose()
        {
            try
            {
                if (!string.IsNullOrEmpty(_tmpfile) && File.Exists(_tmpfile))
                    File.Delete(_tmpfile);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex, "TempFile.Dispose");
            }
        }
    }

    public class TempFolder : IDisposable
    {
        private readonly string tempFolder;

        public TempFolder()
        {
            tempFolder = Path.GetTempFileName();
            // delete old tmp-File
            File.Delete(tempFolder);

            // create a temp folder
            Directory.CreateDirectory(tempFolder);
        }

        public string FolderName
        {
            get { return tempFolder; }
        }

        void IDisposable.Dispose()
        {
            try
            {
                if (!string.IsNullOrEmpty(tempFolder) && Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "TempFolder.Dispose");
            }
        }
    }
}
