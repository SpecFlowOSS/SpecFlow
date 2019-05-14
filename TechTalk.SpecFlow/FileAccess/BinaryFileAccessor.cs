using System.IO;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.FileAccess
{
    public class BinaryFileAccessor : IBinaryFileAccessor
    {
        public Result<Stream> OpenAppendOrCreateFile(string filePath)
        {
            try
            {
                var streamToReturn = File.Open(filePath, FileMode.Append, System.IO.FileAccess.Write, FileShare.Read);
                return Result<Stream>.Success(streamToReturn);
            }
            catch (IOException)
            {
                return Result<Stream>.Failure();
            }
        }
    }
}
