using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class ReplaceTokenInFileTask : Task
    {

        [Required] public string InputFile { get; set; }
        [Required] public string OutputFile { get; set; }
        [Required] public string TextToReplace { get; set; }
        [Required] public string TextToReplaceWith { get; set; }
        public override bool Execute()
        {
            try
            {
                var fileContent = File.ReadAllText(InputFile);
                var replacedContent = fileContent.Replace(TextToReplace, TextToReplaceWith);
                var dir = Path.GetDirectoryName(OutputFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(OutputFile, replacedContent);

                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }
    }
}