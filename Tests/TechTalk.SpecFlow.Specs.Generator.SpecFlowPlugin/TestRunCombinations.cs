using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    class TestRunCombinations
    {
        public static List<Combination> List { get; } = new List<Combination>()
        {
#if XUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "Old", TargetFramework = "Net452", UnitTestProvider = "XUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = "Net452", UnitTestProvider = "XUnit", ConfigFormat = "Config"},
            //new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = "Netcoreapp20", UnitTestProvider = "XUnit"},
#endif

#if MSTEST_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = "Net452", UnitTestProvider = "MSTest", ConfigFormat = "Config"},
            //new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = "Netcoreapp20", UnitTestProvider = "MSTest"},
#endif

#if NUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = "Net452", UnitTestProvider = "NUnit3", ConfigFormat = "Config"},
            //new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = "Netcoreapp20", UnitTestProvider = "NUnit3"},
#endif
        };
    }
}
