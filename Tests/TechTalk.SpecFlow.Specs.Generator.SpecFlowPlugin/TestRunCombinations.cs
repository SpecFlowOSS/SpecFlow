using System.Collections.Generic;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    public class TestRunCombinations
    {
        public const string TfmEnumValueNet452 = "Net452";
        public const string TfmEnumValueNetCore21 = "Netcoreapp21";
        public const string TfmEnumValueNetCore30 = "Netcoreapp30";

        public static List<Combination> List { get; } = new List<Combination>()
        {
#if XUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "Old", TargetFramework = TfmEnumValueNet452, UnitTestProvider = "xUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNet452, UnitTestProvider = "xUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore21, UnitTestProvider = "xUnit", ConfigFormat = "Json"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore30, UnitTestProvider = "xUnit", ConfigFormat = "Json"},
#endif

#if MSTEST_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNet452, UnitTestProvider = "MSTest", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore21, UnitTestProvider = "MSTest", ConfigFormat = "Json"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore30, UnitTestProvider = "MSTest", ConfigFormat = "Json"},
#endif

#if NUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNet452, UnitTestProvider = "NUnit3", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore21, UnitTestProvider = "NUnit3", ConfigFormat = "Json"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore30, UnitTestProvider = "NUnit3", ConfigFormat = "Json"},
#endif
        };
    }
}
