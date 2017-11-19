using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SpecFlow.TestProjectGenerator
{
    public static class CurrentVersionDriver
    {
        static CurrentVersionDriver()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var gitVersionInformationType = assembly.GetType(assemblyName + ".GitVersionInformation");
            var fields = gitVersionInformationType.GetFields();
            Major = (string)gitVersionInformationType?.GetField("Major")?.GetValue(null);
            Minor = (string)gitVersionInformationType?.GetField("Minor")?.GetValue(null);
            Patch = (string)gitVersionInformationType?.GetField("Patch")?.GetValue(null);
            NuGetVersion = (string)gitVersionInformationType?.GetField("NuGetVersion")?.GetValue(null);

            foreach (var field in fields)
            {
                Debug.WriteLine(string.Format("{0}: {1}", field.Name, field.GetValue(null)));
            }


            var specFlowAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "TechTalk.SpecFlow").SingleOrDefault();
            if (specFlowAssembly != null)
            {
                var specFlowVersion = specFlowAssembly.GetName().Version;

                SpecFlowMajor = specFlowVersion.Major;
                SpecFlowMinor = specFlowVersion.Minor;

                SpecFlowVersion = $"{specFlowVersion.Major}.{specFlowVersion.Minor}.0";
                SpecFlowVersionDash = $"{specFlowVersion.Major}-{specFlowVersion.Minor}-0";
            }
        }

        public static string SpecFlowVersionDash { get; private set; }

        public static string Major { get; private set; }
        public static string Minor { get; private set; }
        public static string Patch { get; private set; }
        public static string NuGetVersion { get; private set; }

        public static string SpecFlowVersion { get; private set; }
        public static int SpecFlowMajor { get; set; }
        public static int SpecFlowMinor { get; set; }
    }
}
