using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IProjectCodeBehindGenerator
    {
        IReadOnlyCollection<ITaskItem> GenerateCodeBehindFilesForProject();
    }
}
