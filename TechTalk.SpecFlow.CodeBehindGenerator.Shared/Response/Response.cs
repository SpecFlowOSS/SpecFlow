using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response
{
    public class Response : BaseResponse
    {
        public string Type { get; set; }
        public string Method { get; set; }
        public string Result { get; set; }
    }
}
