using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response
{
    public abstract class BaseResponse
    {
    }

    public class ShutdownResponse : BaseResponse
    {
        public ShutdownResponse(int id)
        {
            ServerProcessId = id;
        }

        public int ServerProcessId { get; set; }
    }

    public class InitProjectResponse : BaseResponse
    {
        public string SessionId { get; set; }
    }

    public class GenerateCodeBehindResponse : BaseResponse
    {
        public string Content { get; set; }
    }

    public class RejectedBuildResponse : BaseResponse
    {
        
    }
}
