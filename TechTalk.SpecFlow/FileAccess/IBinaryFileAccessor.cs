using System.IO;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.FileAccess
{
    public interface IBinaryFileAccessor
    {
        Result<Stream> OpenAppendOrCreateFile(string filePath);
    }
}
