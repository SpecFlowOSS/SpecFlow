using System.Collections.Generic;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    public class TestRunCombinations
    {
        public const string TfmEnumValuenet462 = "Net462";
        public const string TfmEnumValueNetCore21 = "Netcoreapp21";
        public const string TfmEnumValueNetCore31 = "Netcoreapp31";
        public const string TfmEnumValueNet50 = "Net50";
        public const string TfmEnumValueNet60 = "Net60";

        public static List<Combination> List { get; } = new List<Combination>()
        {
#if XUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "Old", TargetFramework = TfmEnumValuenet462, UnitTestProvider = "xUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValuenet462, UnitTestProvider = "xUnit", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore31, UnitTestProvider = "xUnit", ConfigFormat = "Json"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNet60, UnitTestProvider = "xUnit", ConfigFormat = "Json"},
#endif

#if MSTEST_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValuenet462, UnitTestProvider = "MSTest", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore31, UnitTestProvider = "MSTest", ConfigFormat = "Json"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNet60, UnitTestProvider = "MSTest", ConfigFormat = "Json"},
#endif

#if NUNIT_SPECS
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValuenet462, UnitTestProvider = "NUnit3", ConfigFormat = "Config"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNetCore31, UnitTestProvider = "NUnit3", ConfigFormat = "Json"},
            new Combination() {ProgrammingLanguage = "CSharp", ProjectFormat = "New", TargetFramework = TfmEnumValueNet60, UnitTestProvider = "NUnit3", ConfigFormat = "Json"},
#endif
        };
    }
}
