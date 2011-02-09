using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CSharp;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace ParserTests
{
	[TestFixture]
	public class SuccessfulXUnitGenerationTest
	{
		[Test]
		public void CanGenerateFromFiles()
		{
			foreach (var testFile in TestFileHelper.GetTestFiles())
			{
				CanGenerateFromFile(testFile);
			}
		}

		[Test]
		public void CanGenerateSimpleFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "simple.feature"));
		}

		[Test]
		public void CanGenerateCommentsFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "comments.feature"));
		}

		[Test]
		public void CanGenerateFeatureheaderFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "featureheader.feature"));
		}

		[Test]
		public void CanGenerateTagsFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "tags.feature"));
		}

		[Test]
		public void CanGeneratebackgroundFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "background.feature"));
		}

		[Test]
		public void CanGeneratebackgroundWithTitleFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "background_withtitle.feature"));
		}

		[Test]
		public void CanGenerateWhitespacesFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "whitespaces.feature"));
		}

		[Test]
		public void CanGenerateGivenWhenThenDuplicationFeatureFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "givenwhenthenduplication.feature"));
		}

		[Test]
		public void CanGenerateButFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "but.feature"));
		}

		[Test]
		public void CanGenerateMultilinetitleFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "multilinetitle.feature"));
		}

		[Test]
		public void CanGenerateMultilineargumentFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "multilineargument.feature"));
		}

		[Test]
		public void CanGenerateTableargumentFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "tableargument.feature"));
		}

		[Test]
		public void CanGenerateScneriooutlineFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "scenariooutline.feature"));
		}

		[Test]
		public void CanGenerateMixedGWTFeature()
		{
			var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
			CanGenerateFromFile(Path.Combine(folder, "mixedgivenwhenthen.feature"));
		}

		public void CanGenerateFromFile(string fileName)
		{
			Console.WriteLine(fileName);
			SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
			using (var reader = new StreamReader(fileName))
			{
				Feature feature = parser.Parse(reader, null);
				Assert.IsNotNull(feature);

				string generatedCode = GenerateCodeFromFeature(feature);
				Assert.IsNotNull(generatedCode);

				// to regenerate the expected result file:
				//GenerateCodeFromFeature(feature, fileName + ".cs");

				//CompareWithExpectedResult(feature, fileName + ".cs");
			}
		}

		private void CompareWithExpectedResult(Feature feature, string expectedResultFileName)
		{
			string expected = TestFileHelper.ReadFile(expectedResultFileName);
			string got = GenerateCodeFromFeature(feature);

			Assert.AreEqual(expected, got);
		}

		private void GenerateCodeFromFeature(Feature feature, TextWriter writer)
		{
		    CodeDomHelper codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
		    XUnitTestGeneratorProvider xUnitTestGeneratorProvider = new XUnitTestGeneratorProvider();
            SpecFlowUnitTestConverter converter = new SpecFlowUnitTestConverter(xUnitTestGeneratorProvider, codeDomHelper, true, true);
			var codeNamespace = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

			CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			codeProvider.GenerateCodeFromNamespace(codeNamespace, writer, options);
		}

		private void GenerateCodeFromFeature(Feature feature, string fileName)
		{
			using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
			{
				GenerateCodeFromFeature(feature, writer);
			}
		}

		private string GenerateCodeFromFeature(Feature feature)
		{
			using (var writer = new Utf8StringWriter())
			{
				GenerateCodeFromFeature(feature, writer);
				return writer.ToString();
			}
		}
	}
}
