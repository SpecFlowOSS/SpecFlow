using System.Collections.Generic;

namespace TechTalk.SpecFlow.Rpc.Client
{
    public class InterfaceMethodInfo
    {
        public InterfaceMethodInfo(string assembly, string typename, string methodname, Dictionary<int, object> arguments)
        {
            Assembly = assembly;
            Typename = typename;
            Methodname = methodname;
            Arguments = arguments;
        }

        public string Assembly { get; private set; }
        public string Typename { get; private set; }
        public string Methodname { get; private set; }
        public Dictionary<int, object> Arguments { get; private set; }

    }
}