using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public class FileAssociationDetector
    {
        [DllImport("shell32.dll")]
        static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        public bool IsAssociated()
        {
            throw new NotImplementedException();
        }
    }
}
