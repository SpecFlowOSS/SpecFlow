using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    class TestRunCombinations
    {
        public const string TFM_FullFramework = "Net452";
        public const string TFM_NetCore = "Netcoreapp20";

        public static List<Combination> List { get; } = new List<Combination>()
        {
#if XUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "Old", TargetFramework = TFM_FullFramework, UnitTestProvider = "XUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TFM_FullFramework, UnitTestProvider = "XUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TFM_NetCore, UnitTestProvider = "XUnit", ConfigFormat = "Json"},
#endif

#if MSTEST_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TFM_FullFramework, UnitTestProvider = "MSTest", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TFM_NetCore, UnitTestProvider = "MSTest", ConfigFormat = "Json"},
#endif

#if NUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TFM_FullFramework, UnitTestProvider = "NUnit3", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TFM_NetCore, UnitTestProvider = "NUnit3", ConfigFormat = "Json"},
#endif
        };
    }
}
