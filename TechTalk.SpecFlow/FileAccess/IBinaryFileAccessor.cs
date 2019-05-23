using System.IO;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.FileAccess
{
    public interface IBinaryFileAccessor
    {
        IResult<Stream> OpenAppendOrCreateFile(string filePath);
    }
}
