using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    class TestRunCombinations
    {
        public const string TfmNet452 = "net452";
        public const string TfmNetCore21 = "netcoreapp2.1";

        public static List<Combination> List { get; } = new List<Combination>()
        {
#if XUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "Old", TargetFramework = TfmNet452, UnitTestProvider = "xUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmNet452, UnitTestProvider = "xUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmNetCore21, UnitTestProvider = "xUnit", ConfigFormat = "Json"},
#endif

#if MSTEST_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmNet452, UnitTestProvider = "MSTest", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmNetCore21, UnitTestProvider = "MSTest", ConfigFormat = "Json"},
#endif

#if NUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmNet452, UnitTestProvider = "NUnit3", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmNetCore21, UnitTestProvider = "NUnit3", ConfigFormat = "Json"},
#endif
        };
    }
}
