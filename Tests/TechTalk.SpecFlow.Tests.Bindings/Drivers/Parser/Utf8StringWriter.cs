using System.IO;
using System.Text;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers.Parser
{
    internal class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}