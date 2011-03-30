using System;
using System.IO;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace GeneratorTests
{
    public abstract class TestGeneratorTestsBase
    {
        protected GenerationSettings net35CSSettings;
        protected GenerationSettings net35VBSettings;

        [SetUp]
        public virtual void Setup()
        {
            net35CSSettings = new GenerationSettings
                                  {
                                      TargetLanguage = GenerationTargetLanguage.CSharp,
                                      TargetLanguageVersion = new Version("3.0"),
                                      TargetPlatform = GenerationTargetPlatform.DotNet,
                                      TargetPlatformVersion = new Version("3.5"),
                                      ProjectDefaultNamespace = "DefaultNamespace",
                                      CheckUpToDate = false
                                  };
            net35VBSettings = new GenerationSettings
                                  {
                                      TargetLanguage = GenerationTargetLanguage.VB,
                                      TargetLanguageVersion = new Version("9.0"),
                                      TargetPlatform = GenerationTargetPlatform.DotNet,
                                      TargetPlatformVersion = new Version("3.5"),
                                      ProjectDefaultNamespace = "DefaultNamespace",
                                      CheckUpToDate = false
                                  };
        }

        protected FeatureFileInput CreateSimpleValidFeatureFileInput()
        {
            return CreateSimpleFeatureFileInput(@"
Feature: Addition

@mytag
Scenario: Add two numbers
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
");
        }

        protected FeatureFileInput CreateSimpleFeatureFileInput(string featureFileContent)
        {
            return new FeatureFileInput(
                @"C:\Temp\Dummy.feature",
                @"Dummy.feature",
                null,
                new StringReader(featureFileContent)
                );
        }

        protected FeatureFileInput CreateSimpleInvalidFeatureFileInput()
        {
            return CreateSimpleFeatureFileInput(@"
Feature: Addition
Scenario: Add two numbers
	Given I have entered 50 into the calculator
    AndXXX the keyword is misspelled
");
        }
    }
}