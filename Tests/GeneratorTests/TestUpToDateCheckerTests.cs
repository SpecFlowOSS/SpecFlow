﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;
using Should;

namespace GeneratorTests
{
    [TestFixture]
    public class TestUpToDateCheckerTests
    {
        protected Mock<ITestHeaderWriter> TestHeaderWriterStub;

        [SetUp]
        public virtual void Setup()
        {
            TestHeaderWriterStub = new Mock<ITestHeaderWriter>();
        }

        private TestUpToDateChecker CreateUpToDateChecker()
        {
            var net35CSSettings = new ProjectPlatformSettings
            {
                Language = GenerationTargetLanguage.CSharp,
                LanguageVersion = new Version("3.0"),
                Platform = GenerationTargetPlatform.DotNet,
                PlatformVersion = new Version("3.5"),
            };

            return new TestUpToDateChecker(TestHeaderWriterStub.Object, 
                new GeneratorInfo { GeneratorVersion = TestGeneratorFactory.GeneratorVersion}, 
                new ProjectSettings { ProjectFolder = Path.GetTempPath(), ProjectPlatformSettings = net35CSSettings });
        }

        [Test]
        public void Should_detect_up_to_date_test_file_based_on_modification_time()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_code");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(TestGeneratorFactory.GeneratorVersion);

                    var result = testUpToDateChecker.IsUpToDatePreliminary(new FeatureFileInput(tempFile.FileName),
                        tempTestFile.FullPath, UpToDateCheckingMethod.ModificationTimeAndGeneratorVersion);

                    result.ShouldEqual(true);
                }
            }
        }

        [Test]
        public void Should_detect_outdated_date_test_file_if_feature_file_has_outdated_generator_version()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_code");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    this.TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(new Version(1, 0));
                    // version 1.0 is surely older than the current one

                    var result = testUpToDateChecker.IsUpToDatePreliminary(new FeatureFileInput(tempFile.FileName),
                        tempTestFile.FullPath, UpToDateCheckingMethod.ModificationTimeAndGeneratorVersion);

                    result.ShouldEqual(false);
                }
            }
        }

        [Test]
        public void Should_detect_outdated_date_test_file_if_feature_file_missing()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                var testUpToDateChecker = CreateUpToDateChecker();
                var result = testUpToDateChecker.IsUpToDatePreliminary(new FeatureFileInput(tempFile.FileName),
                    tempFile.FileName + ".cs", UpToDateCheckingMethod.ModificationTimeAndGeneratorVersion);

                result.ShouldEqual(false);
            }
        }

        [Test]
        public void Should_detect_outdated_date_test_file_if_feature_file_changed_based_on_modification_time()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_code");

                    //re-set content with a slight delay
                    Thread.Sleep(10);
                    tempFile.SetContent("new_feature");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(TestGeneratorFactory.GeneratorVersion);

                    var result = testUpToDateChecker.IsUpToDatePreliminary(new FeatureFileInput(tempFile.FileName),
                        tempTestFile.FullPath, UpToDateCheckingMethod.ModificationTimeAndGeneratorVersion);

                    result.ShouldEqual(false);
                }
            }
        }

        [Test]
        public void Should_not_give_preliminary_positive_result_if_file_content_check_was_requested()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_code");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(TestGeneratorFactory.GeneratorVersion);

                    var result = testUpToDateChecker.IsUpToDatePreliminary(new FeatureFileInput(tempFile.FileName),
                        tempTestFile.FullPath, UpToDateCheckingMethod.FileContent);

                    result.ShouldBeNull();
                }
            }
        }

        [Test]
        public void Should_detect_up_to_date_test_file_based_on_content_compare_from_file()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_code");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(TestGeneratorFactory.GeneratorVersion);

                    var result = testUpToDateChecker.IsUpToDate(new FeatureFileInput(tempFile.FileName),
                        tempTestFile.FullPath, "any_code", UpToDateCheckingMethod.FileContent);

                    result.ShouldBeTrue();
                }
            }
        }

        [Test]
        public void Should_detect_up_to_date_test_file_based_on_content_compare_from_provided_content()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_old_code");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(TestGeneratorFactory.GeneratorVersion);

                    var result = testUpToDateChecker.IsUpToDate(new FeatureFileInput(tempFile.FileName) { GeneratedTestFileContent = "any_code" },
                        tempTestFile.FullPath, "any_code", UpToDateCheckingMethod.FileContent);

                    result.ShouldBeTrue();
                }
            }
        }

        [Test]
        public void Should_outdated_test_file_based_on_content_compare_from_file()
        {
            using (var tempFile = new TempFile(".sfeature"))
            {
                tempFile.SetContent("any");

                using (var tempTestFile = new TempFile(".cs"))
                {
                    // set test content
                    tempTestFile.SetContent("any_code");

                    var testUpToDateChecker = CreateUpToDateChecker();

                    TestHeaderWriterStub.Setup(thw => thw.DetectGeneratedTestVersion(It.IsAny<string>())).Returns(TestGeneratorFactory.GeneratorVersion);

                    var result = testUpToDateChecker.IsUpToDate(new FeatureFileInput(tempFile.FileName),
                        tempTestFile.FullPath, "new_code", UpToDateCheckingMethod.FileContent);

                    result.ShouldBeFalse();
                }
            }
        }
    }
}
