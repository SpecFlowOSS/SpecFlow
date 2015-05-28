using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class HooksDriver
    {
        private readonly InputProjectDriver inputProjectDriver;

        private Lazy<string> hookLog;

        public string HookLogPath { get; private set; }
        public string HookLog { get { return hookLog.Value; } }

        public HooksDriver(InputProjectDriver inputProjectDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
            HookLogPath = Path.Combine(inputProjectDriver.DeploymentFolder, "hooks.log");

            hookLog = new Lazy<string>(() => File.ReadAllText(HookLogPath));

            //reset file
            File.WriteAllText(HookLogPath, "");
        }

        public string GetHookLogStatement(string methodName)
        {
            return string.Format(@"System.IO.File.AppendAllText(@""{1}"", ""-> hook: {0}"");", methodName, HookLogPath);
        }
    }
}
