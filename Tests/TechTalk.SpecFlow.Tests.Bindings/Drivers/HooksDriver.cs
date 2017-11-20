﻿using System;
using System.IO;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class HooksDriver
    {
        private Lazy<string> hookLog;
        private bool isInitialized = false;

        public string HookLogPath { get; private set; }
        public string HookLog { get { return hookLog.Value; } }

        public HooksDriver(InputProjectDriver inputProjectDriver)
        {
            HookLogPath = Path.Combine(inputProjectDriver.DeploymentFolder, "hooks.log");
            hookLog = new Lazy<string>(() =>
            {
                EnsureInitialized();
                return File.ReadAllText(HookLogPath);
            });
        }

        public string GetHookLogStatement(string methodName)
        {
            //EnsureInitialized();
            return string.Format(@"System.IO.File.AppendAllText(@""{1}"", ""-> hook: {0}"");", methodName, HookLogPath);
        }

        public void EnsureInitialized()
        {
            if (isInitialized)
                return;
            
            //reset file
            File.WriteAllText(HookLogPath, "");
            isInitialized = true;
        }
    }
}
