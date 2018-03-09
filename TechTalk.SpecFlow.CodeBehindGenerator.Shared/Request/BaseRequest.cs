using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request
{
    public abstract class BaseRequest
    {
    }

    public class ShutdownRequest : BaseRequest
    {
        
    }

    public class InitProjectRequest : BaseRequest
    {
        public string ProjectFilePath { get; set; }

    }

    public class GenerateCodeBehindRequest : BaseRequest
    {
        public string SessionId { get; set; }
        public string FeatureFilePath { get; set; }
    }
}
