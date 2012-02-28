﻿using System;
using System.IO;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace GeneratorTests
{
    public abstract class TestGeneratorTestsBase
    {
        protected ProjectPlatformSettings net35CSSettings;
        protected ProjectPlatformSettings net35VBSettings;
        protected ProjectSettings net35CSProjectSettings;
        protected ProjectSettings net35VBProjectSettings;
        protected GenerationSettings defaultSettings;
        protected Mock<ITestHeaderWriter> TestHeaderWriterStub;
        protected Mock<ITestUpToDateChecker> TestUpToDateCheckerStub;

        [SetUp]
        public virtual void Setup()
        {
            net35CSSettings = new ProjectPlatformSettings
                                  {
                                      Language = GenerationTargetLanguage.CSharp,
                                      LanguageVersion = new Version("3.0"),
                                      Platform = GenerationTargetPlatform.DotNet,
                                      PlatformVersion = new Version("3.5"),
                                  };
            net35VBSettings = new ProjectPlatformSettings
                                  {
                                      Language = GenerationTargetLanguage.VB,
                                      LanguageVersion = new Version("9.0"),
                                      Platform = GenerationTargetPlatform.DotNet,
                                      PlatformVersion = new Version("3.5"),
                                  };

            net35CSProjectSettings = new ProjectSettings { ProjectFolder = Path.GetTempPath(), ProjectPlatformSettings = net35CSSettings };
            net35VBProjectSettings = new ProjectSettings { ProjectFolder = Path.GetTempPath(), ProjectPlatformSettings = net35VBSettings };
            defaultSettings = new GenerationSettings();

            TestHeaderWriterStub = new Mock<ITestHeaderWriter>();
            TestUpToDateCheckerStub = new Mock<ITestUpToDateChecker>();
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
            return new FeatureFileInput(@"Dummy.sfeature") {FeatureFileContent = featureFileContent};
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

        protected TestGenerator CreateTestGenerator()
        {
            return CreateTestGenerator(net35CSProjectSettings);
        }

        protected TestGenerator CreateTestGenerator(ProjectSettings projectSettings)
        {
            return new TestGenerator(new GeneratorConfiguration(), projectSettings, TestHeaderWriterStub.Object, TestUpToDateCheckerStub.Object);
        }
    }
}