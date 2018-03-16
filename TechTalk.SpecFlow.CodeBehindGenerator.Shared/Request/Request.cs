using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request
{
    public class Request : BaseRequest
    {
        public string Type { get; set; }
        public string Method { get; set; }
        public string Arguments { get; set; }
    }
}
