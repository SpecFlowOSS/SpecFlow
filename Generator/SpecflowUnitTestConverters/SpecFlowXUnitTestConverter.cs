using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator
{
	public class SpecFlowXUnitTestConverter : ISpecFlowUnitTestConverter
	{
		private const string DEFAULT_NAMESPACE = "SpecFlowTests";
		private const string FEATURE_FORMAT = "{0}Feature";
		private const string TEST_FORMAT = "{0}";
		private const string IGNORE_TAG = "Ignore";

		private const string XUNIT_NAMESPACE="Xunit";
		private const string SPECFLOW_NAMESPACE = "TechTalk.SpecFlow";

		private const string ITESTRUNNER_TYPE = "ITestRunner";
		private const string TESTRUNNER_FIELD_NAME = "_testRunner";
		private const string TESTRUNNER_PROPERTY_NAME = "TestRunner";

		private const string IUSEFIXTURE_TYPE = "IUseFixture";
		private const string FIXTURE_FORMAT = "{0}Fixture";
		private const string SETFIXTURE_METHOD_NAME = "SetFixture";
		private const string SETFIXTURE_METHOD_PARAMETER_NAME = "fixture";
		private const string FIXTURE_FIELD_NAME = "_fixture";

		private const string IDISPOSABLE_TYPE = "IDisposable";
		private const string ISDISPOSED_FIELD_NAME = "_isDisposed";
		private const string DISPOSE_METHOD_NAME = "Dispose";
		private const string ISDISPOSING_PARAMETER_NAME = "isDisposing";

		private const string ON_FEATUREEND_METHOD_NAME = "OnFeatureEnd";
		private const string FEATUREINFO_TYPE = "FeatureInfo";
		private const string BACKGROUND_NAME = "FeatureBackground";

		private const string TESTRUNNERMANAGER_TYPE = "TechTalk.SpecFlow.TestRunnerManager";
		private const string SCENARIOINFO_TYPE = "TechTalk.SpecFlow.ScenarioInfo";
		private const string TABLE_TYPE = "TechTalk.SpecFlow.Table";
	
		private readonly IUnitTestGeneratorProvider _testGeneratorProvider;
		private readonly bool _allowDebugGeneratedFiles;
		private CodeNamespace _codeNamespace;

		public SpecFlowXUnitTestConverter(IUnitTestGeneratorProvider testGeneratorProvider, bool allowDebugGeneratedFiles)
		{
			_testGeneratorProvider = testGeneratorProvider;
			_allowDebugGeneratedFiles = allowDebugGeneratedFiles;
		}

		public CodeNamespace GenerateUnitTestFixture(Feature feature, string testClassName, string targetNamespace)
		{
			targetNamespace = targetNamespace ?? DEFAULT_NAMESPACE;
			testClassName = testClassName ?? string.Format(FEATURE_FORMAT, feature.Title.ToIdentifier());

			_codeNamespace = new CodeNamespace(targetNamespace);

			_codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
			_codeNamespace.Imports.Add(new CodeNamespaceImport("System.Globalization"));
			_codeNamespace.Imports.Add(new CodeNamespaceImport(XUNIT_NAMESPACE));
			_codeNamespace.Imports.Add(new CodeNamespaceImport(SPECFLOW_NAMESPACE));

			var testType = new CodeTypeDeclaration(testClassName)
			{
				IsPartial = true
			};
			testType.TypeAttributes |= TypeAttributes.Public;
			_codeNamespace.Types.Add(testType);

			AddLinePragmaInitial(testType, feature);

			_testGeneratorProvider.SetTestFixture(testType, feature.Title, feature.Description);
			if (feature.Tags != null)
			{
				_testGeneratorProvider.SetTestFixtureCategories(testType, GetNonIgnoreTags(feature.Tags));
				if (feature.Tags.Any(t => t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)))
				{
					_testGeneratorProvider.SetIgnore(testType);
				}
			}

			AdjustTypeForIUseFixture(testType, feature);
			AdjustFeatureForIDisposable(testType);

			var testSetupMethod = GenerateTestSetup(testType);
			if (feature.Background != null)
			{
				GenerateBackground(testType, testSetupMethod, feature.Background);
			}

			GenerateTestTearDown(testType);
			
			foreach (var scenario in feature.Scenarios)
			{
				if (scenario is ScenarioOutline)
				{
					GenerateScenarioOutlineTest(testType, testSetupMethod, (ScenarioOutline)scenario, feature);
				}
				else
				{
					GenerateScenarioTest(testType, testSetupMethod, scenario, feature);
				}
			}
			return _codeNamespace;
		}

		private void AdjustTypeForIUseFixture(CodeTypeDeclaration testType, Feature feature)
		{
			string fixtureName = string.Format(FIXTURE_FORMAT, feature.Title.ToIdentifier());

			GenerateFixture(feature, fixtureName);

			// Inherit IUseFixture<Fixture>
			testType.BaseTypes.Add
			(
				new CodeTypeReference(IUSEFIXTURE_TYPE, new CodeTypeReference[] { new CodeTypeReference(fixtureName) })
			);

			// Add Fixture data;
			testType.Members.Add(new CodeMemberField(fixtureName, FIXTURE_FIELD_NAME));

			// Implement IUseFixture.SetFixture
			CodeMemberMethod setFixtureMethod = new CodeMemberMethod()
			{
				Name = SETFIXTURE_METHOD_NAME,
				Attributes = MemberAttributes.Public | MemberAttributes.Final
			};
			setFixtureMethod.Parameters.Add
			(
				new CodeParameterDeclarationExpression(new CodeTypeReference(fixtureName), SETFIXTURE_METHOD_PARAMETER_NAME)
			);

			// _fixture = fixture;
			setFixtureMethod.Statements.Add
			(
				new CodeAssignStatement
				(
					new CodeVariableReferenceExpression(FIXTURE_FIELD_NAME),
					new CodeArgumentReferenceExpression(SETFIXTURE_METHOD_PARAMETER_NAME)
				)
			);
			testType.Members.Add(setFixtureMethod);
		}

		private void GenerateFixture(Feature feature, string fixtureName)
		{
			fixtureName = fixtureName ?? string.Format(FIXTURE_FORMAT, feature.Title.ToIdentifier());

			var fixtureType =
				new CodeTypeDeclaration(fixtureName)
				{
					IsPartial = true
				};
			fixtureType.TypeAttributes |= TypeAttributes.Public;

			// Inherit IDisposable
			fixtureType.BaseTypes.Add
			(
				new CodeTypeReference(IDISPOSABLE_TYPE)
			);

			fixtureType.Members.AddRange
			(
				new CodeTypeMember[] 
				{
					// add private bool _isDisposed = false;
					new CodeMemberField(typeof(bool), ISDISPOSED_FIELD_NAME)
					{
						InitExpression = new CodePrimitiveExpression(false)
					},
					// add private ITestRunner _testRunner;
					new CodeMemberField(ITESTRUNNER_TYPE, TESTRUNNER_FIELD_NAME)
					{
						Attributes = MemberAttributes.Private
					}
				}
			);

			CodeMemberProperty testRunnerProperty =
				new CodeMemberProperty()
				{
					Name = TESTRUNNER_PROPERTY_NAME,
					Attributes = MemberAttributes.Public | MemberAttributes.Final,
					Type = new CodeTypeReference(ITESTRUNNER_TYPE)
				};
			testRunnerProperty.GetStatements.Add
			(
				new CodeMethodReturnStatement
				(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD_NAME)
				)
			);
			fixtureType.Members.Add(testRunnerProperty);

			CodeMemberMethod disposeMethod;

			// add public void Dispose()
			disposeMethod = new CodeMemberMethod()
			{
				Name = DISPOSE_METHOD_NAME,
				Attributes = MemberAttributes.Public | MemberAttributes.Final
			};
			disposeMethod.Statements.AddRange
			(
				new CodeStatement[]
				{
					new CodeExpressionStatement
					(
						new CodeMethodInvokeExpression
						(
							new CodeThisReferenceExpression(),
							DISPOSE_METHOD_NAME,
							new CodePrimitiveExpression(true)
						)
					),
					new CodeExpressionStatement
					(
						new CodeMethodInvokeExpression
						(
							new CodeTypeReferenceExpression("GC"),
							"SuppressFinalize",
							new CodeThisReferenceExpression()
						)
					)
				}
			);
			fixtureType.Members.Add(disposeMethod);

			// add private void Dispose(bool isDisposing)
			disposeMethod = new CodeMemberMethod()
			{
				Name = DISPOSE_METHOD_NAME,
				Attributes = MemberAttributes.Private | MemberAttributes.Final
			};
			disposeMethod.Parameters.Add
			(
				new CodeParameterDeclarationExpression(typeof(bool), ISDISPOSING_PARAMETER_NAME)
			);
			disposeMethod.Statements.Add
			(
				new CodeConditionStatement
				(
					new CodeBinaryOperatorExpression
					(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ISDISPOSED_FIELD_NAME),
						CodeBinaryOperatorType.ValueEquality,
						new CodePrimitiveExpression(false)
					),
					new CodeStatement[]
					{
						new CodeConditionStatement
						(
							new CodeBinaryOperatorExpression
							(
						    new CodeBinaryOperatorExpression
								(
									new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD_NAME),
									CodeBinaryOperatorType.ValueEquality,
									new CodePrimitiveExpression(null)
								),
								CodeBinaryOperatorType.ValueEquality,
								new CodePrimitiveExpression(false)
							),
							new CodeStatement[]
							{
								new CodeExpressionStatement
								(
									new CodeMethodInvokeExpression
									(
										new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD_NAME),
										ON_FEATUREEND_METHOD_NAME,
										new CodeExpression[] { }
									)
								),
								new CodeAssignStatement
								(
									new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD_NAME),
									new CodePrimitiveExpression(null)
								)
							}
						),
						new CodeConditionStatement
						(
							new CodeBinaryOperatorExpression
							(
								new CodeVariableReferenceExpression(ISDISPOSING_PARAMETER_NAME),
								CodeBinaryOperatorType.ValueEquality,
								new CodePrimitiveExpression(true)
							),
							new CodeStatement[]
							{
								new CodeExpressionStatement
								(
									new CodeMethodInvokeExpression
									(
										new CodeThisReferenceExpression(),
										"OnDisposeManagedResources",
										new CodeExpression[] { }
									)
								)
							}
						),
						new CodeExpressionStatement
						(
							new CodeMethodInvokeExpression
							(
								new CodeThisReferenceExpression(),
								"OnDisposeUnmanagedResources",
								new CodeExpression[] { } 
							)
						),
						new CodeAssignStatement
						(
							new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ISDISPOSED_FIELD_NAME),
							new CodePrimitiveExpression(true)
						)
					}
				)
			);
			fixtureType.Members.Add(disposeMethod);

			// add destructor
			fixtureType.Members.Add
			(
				new CodeSnippetTypeMember
				(
					string.Format
					(
@"		~{0}()
		{{
			Dispose(false);
		}}",
						fixtureName
					)
				)
			);

			fixtureType.Members.AddRange
			(
				new CodeTypeMember[]
				{
					// add partial void OnDisposeManagedResources()
					new CodeSnippetTypeMember("		partial void OnDisposeManagedResources();"),
					// add partial void OnDisposeUnmanagedResources()
					new CodeSnippetTypeMember("		partial void OnDisposeUnmanagedResources();")
				}
			);

			// add constructor
			CodeConstructor constructor = new CodeConstructor()
			{
				Attributes = MemberAttributes.Public,
				Name = fixtureName
			};
			constructor.Statements.AddRange
			(
				new CodeStatement[]
				{
					new CodeVariableDeclarationStatement
					(
						FEATUREINFO_TYPE,
						"featureInfo",
						new CodeObjectCreateExpression
						(
							FEATUREINFO_TYPE,
							new CodeObjectCreateExpression(typeof(CultureInfo),	new CodePrimitiveExpression(feature.Language)),
							new CodePrimitiveExpression(feature.Title),
							new CodePrimitiveExpression(feature.Description),
							GetStringArrayExpression(feature.Tags)
						)
					),
					new CodeAssignStatement
					(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD_NAME),
						new CodeMethodInvokeExpression
						(
							new CodeTypeReferenceExpression(TESTRUNNERMANAGER_TYPE),
							"GetTestRunner",
							new CodeExpression[] { }
						)
					),
					new CodeExpressionStatement
					(
						new CodeMethodInvokeExpression
						(
							new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD_NAME),
							"OnFeatureStart",
							new CodeVariableReferenceExpression("featureInfo")
						)
					)
				}
			);
			fixtureType.Members.Add(constructor);

			_codeNamespace.Types.Add(fixtureType);
		}

		private void AdjustFeatureForIDisposable(CodeTypeDeclaration testType)
		{
			// Inherit IDisposable
			testType.BaseTypes.Add
			(
				new CodeTypeReference(IDISPOSABLE_TYPE)
			);
		
			// add bool _isDisposed = false;
			testType.Members.Add
			(
				new CodeMemberField(typeof(bool), ISDISPOSED_FIELD_NAME)
				{
					Attributes = MemberAttributes.Private,
					InitExpression = new CodePrimitiveExpression(false)
				}
			);

			CodeMemberMethod disposeMethod;

			// add public void Dispose()
			disposeMethod = new CodeMemberMethod()
			{
				Name = DISPOSE_METHOD_NAME,
				Attributes = MemberAttributes.Public | MemberAttributes.Final
			};
			disposeMethod.Statements.AddRange
			(
				new CodeStatement[]
				{
					new CodeExpressionStatement
					(
						new CodeMethodInvokeExpression
						(
							new CodeThisReferenceExpression(),
							DISPOSE_METHOD_NAME,
							new CodePrimitiveExpression(true)
						)
					),
					new CodeExpressionStatement
					(
						new CodeMethodInvokeExpression
						(
							new CodeTypeReferenceExpression("GC"),
							"SuppressFinalize",
							new CodeThisReferenceExpression()
						)
					)
				}
			);
			testType.Members.Add(disposeMethod);

			// add private void Dispose(bool isDisposing)
			disposeMethod = new CodeMemberMethod()
			{
				Name = DISPOSE_METHOD_NAME,
				Attributes = MemberAttributes.Private | MemberAttributes.Final
			};
			disposeMethod.Parameters.Add
			(
				new CodeParameterDeclarationExpression(typeof(bool), ISDISPOSING_PARAMETER_NAME)
			);
			disposeMethod.Statements.Add
			(
				new CodeConditionStatement
				(
					new CodeBinaryOperatorExpression
					(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ISDISPOSED_FIELD_NAME),
						CodeBinaryOperatorType.ValueEquality,
						new CodePrimitiveExpression(false)
					),
					new CodeStatement[]
					{
						new CodeConditionStatement
						(
							new CodeBinaryOperatorExpression
							(
								new CodeVariableReferenceExpression(ISDISPOSING_PARAMETER_NAME),
								CodeBinaryOperatorType.ValueEquality,
								new CodePrimitiveExpression(true)
							),
							new CodeStatement[]
							{
								new CodeExpressionStatement
								(
									new CodeMethodInvokeExpression
									(
										new CodeThisReferenceExpression(),
										"OnDisposeManagedResources",
										new CodeExpression[] { }
									)
								)
							}
						),
						new CodeExpressionStatement
						(
							new CodeMethodInvokeExpression
							(
								new CodeThisReferenceExpression(),
								"OnDisposeUnmanagedResources",
								new CodeExpression[] { } 
							)
						),
						new CodeAssignStatement
						(
							new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ISDISPOSED_FIELD_NAME),
							new CodePrimitiveExpression(true)
						)
					}
				)
			);
			testType.Members.Add(disposeMethod);

			// add destructor
			testType.Members.Add
			(
				new CodeSnippetTypeMember
				(
					string.Format
					(
@"		~{0}()
		{{
			Dispose(false);
		}}",
						testType.Name
					)
				)
			);

			testType.Members.AddRange
			(
				new CodeTypeMember[]
				{
					// add partial void OnDisposeManagedResources()
					new CodeSnippetTypeMember("		partial void OnDisposeManagedResources();"),
					// add partial void OnDisposeUnmanagedResources()
					new CodeSnippetTypeMember("		partial void OnDisposeUnmanagedResources();")
				}
			);
		}

		private CodeMemberMethod GenerateTestSetup(CodeTypeDeclaration testType)
		{
			CodeConstructor constructor = new CodeConstructor()
			{
				Attributes = MemberAttributes.Public
			};
			constructor.Statements.Add
			(
				new CodeMethodInvokeExpression
				(
					new CodeThisReferenceExpression(),
					"OnInitialize",
					new CodeExpression[] { }
				)
			);
			testType.Members.Add(constructor);

			testType.Members.Add
			(
				// add partial void OnInitialize()
				new CodeSnippetTypeMember("		partial void OnInitialize();")
			);

			return constructor;
		}

		private void GenerateBackground(CodeTypeDeclaration testType, CodeMemberMethod testSetup, Background background)
		{
			CodeMemberMethod backgroundMethod = new CodeMemberMethod()
			{
				Attributes = MemberAttributes.Public,
				Name = BACKGROUND_NAME
			};
			testType.Members.Add(backgroundMethod);

			AddLineDirective(backgroundMethod.Statements, background);

			foreach (var given in background.Steps)
			{
				GenerateStep(backgroundMethod, given, null);
			}

			AddLineDirectiveHidden(backgroundMethod.Statements);

			testSetup.Statements.Add
			(
				new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), backgroundMethod.Name)
			);
		}

		private void GenerateTestTearDown(CodeTypeDeclaration testType)
		{
			// XUnit does not implement test teardowns
			// IDisposable was implemented previously
		}

		private void GenerateScenarioOutlineTest(CodeTypeDeclaration testType, CodeMemberMethod testSetup, ScenarioOutline scenarioOutline, Feature feature)
		{
			string testMethodName = string.Format(TEST_FORMAT, scenarioOutline.Title.ToIdentifier());

			ParameterSubstitution paramToIdentifier = new ParameterSubstitution();
			foreach (var param in scenarioOutline.Examples.ExampleSets[0].Table.Header.Cells)
			{
				paramToIdentifier.Add(param.Value, param.Value.ToIdentifierCamelCase());
			}

			if (scenarioOutline.Examples.ExampleSets.Length > 1)
			{
				//TODO: check params
			}

			GenerateScenarioOutlineBody(feature, scenarioOutline, paramToIdentifier, testType, testMethodName, testSetup);

			int exampleSetIndex = 0;
			foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
			{
				string exampleSetTitle = exampleSet.Title == null ? string.Format("Scenarios{0}", exampleSetIndex + 1) : exampleSet.Title.ToIdentifier();

				bool useFirstColumnAsName = CanUseFirstColumnAsName(exampleSet.Table);

				for (int rowIndex = 0; rowIndex < exampleSet.Table.Body.Length; rowIndex++)
				{
					string variantName = useFirstColumnAsName ? exampleSet.Table.Body[rowIndex].Cells[0].Value.ToIdentifierPart() : string.Format("Variant{0}", rowIndex);
					GenerateScenarioOutlineTestVariant(testType, scenarioOutline, testMethodName, paramToIdentifier, exampleSetTitle, exampleSet.Table.Body[rowIndex], variantName);
				}
				exampleSetIndex++;
			}
		}

		private void GenerateScenarioOutlineBody(Feature feature, ScenarioOutline scenarioOutline, ParameterSubstitution paramToIdentifier, CodeTypeDeclaration testType, string testMethodName, CodeMemberMethod testSetup)
		{
			CodeMemberMethod testMethod = new CodeMemberMethod()
			{
				Attributes = MemberAttributes.Public | MemberAttributes.Final,
				Name = testMethodName
			};
			testType.Members.Add(testMethod);

			foreach (var pair in paramToIdentifier)
			{
				testMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), pair.Value));
			}

			GenerateTestBody(feature, scenarioOutline, testMethod, testSetup, paramToIdentifier);
		}

		private void GenerateScenarioOutlineTestVariant(CodeTypeDeclaration testType, ScenarioOutline scenarioOutline, string testMethodName, List<KeyValuePair<string, string>> paramToIdentifier, string exampleSetTitle, Row row, string variantName)
		{
			CodeMemberMethod testMethod = GetTestMethodDeclaration(testType, scenarioOutline);
			testMethod.Name = string.IsNullOrEmpty(exampleSetTitle) ? string.Format("{0}_{1}", testMethod.Name, variantName) : string.Format("{0}_{1}_{2}", testMethod.Name, exampleSetTitle, variantName);

			//call test implementation with the params
			List<CodeExpression> argumentExpressions = new List<CodeExpression>();
			foreach (var paramCell in row.Cells)
			{
				argumentExpressions.Add(new CodePrimitiveExpression(paramCell.Value));
			}
			testMethod.Statements.Add
			(
				new CodeMethodInvokeExpression
				(
					new CodeThisReferenceExpression(),
					testMethodName,
					argumentExpressions.ToArray()
				)
			);
		}

		private void GenerateScenarioTest(CodeTypeDeclaration testType, CodeMemberMethod testSetup, Scenario scenario, Feature feature)
		{
			CodeMemberMethod testMethod = GetTestMethodDeclaration(testType, scenario);
			GenerateTestBody(feature, scenario, testMethod, testSetup, null);
		}

		private void GenerateTestBody(Feature feature, Scenario scenario, CodeMemberMethod testMethod, CodeMemberMethod testSetup, ParameterSubstitution paramToIdentifier)
		{
			//ScenarioInfo scenarioInfo = new ScenarioInfo("xxxx", tags...);
			testMethod.Statements.Add
			(
				new CodeVariableDeclarationStatement
				(
					SCENARIOINFO_TYPE,
					"scenarioInfo",
					new CodeObjectCreateExpression
					(
						SCENARIOINFO_TYPE,
						new CodePrimitiveExpression(scenario.Title), GetStringArrayExpression(scenario.Tags)
					)
				)
			);

			AddLineDirective(testMethod.Statements, scenario);

			CodePropertyReferenceExpression testRunnerProperty =
				new CodePropertyReferenceExpression
				(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), FIXTURE_FIELD_NAME),
					TESTRUNNER_PROPERTY_NAME
				);

			// _fixture.TestRunner.OnScenarioStart(scenarioInfo);
			testMethod.Statements.Add
			(
				new CodeMethodInvokeExpression
				(
					testRunnerProperty,
					"OnScenarioStart",
					new CodeVariableReferenceExpression("scenarioInfo")
				)
			);

			foreach (var scenarioStep in scenario.Steps)
			{
				// TODO
				GenerateStep(testMethod, scenarioStep, paramToIdentifier);
			}

			AddLineDirectiveHidden(testMethod.Statements);

			// _fixture.TestRunner.CollectScenarioErrors();
			testMethod.Statements.Add
			(
				new CodeMethodInvokeExpression
				(
					testRunnerProperty,
					"CollectScenarioErrors",
					new CodeExpression[] { }
				)
			);

			// _fixture.TestRunner.OnScenarioEnd();
			testMethod.Statements.Add
			(
				new CodeMethodInvokeExpression
				(
					testRunnerProperty,
					"OnScenarioEnd",
					new CodeExpression[] { }
				)
			);
		}

		private void GenerateStep(CodeMemberMethod testMethod, ScenarioStep scenarioStep, ParameterSubstitution paramToIdentifier)
		{
			CodePropertyReferenceExpression testRunnerProperty =
				new CodePropertyReferenceExpression
				(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), FIXTURE_FIELD_NAME),
					TESTRUNNER_PROPERTY_NAME
				);

			// _fixture.TestRunner.Given("something");
			List<CodeExpression> arguments = new List<CodeExpression>();
			arguments.Add(GetSubstitutedString(scenarioStep.Text, paramToIdentifier));
			if (scenarioStep.MultiLineTextArgument != null || scenarioStep.TableArg != null)
			{
				AddLineDirectiveHidden(testMethod.Statements);
				arguments.Add(GetMultilineTextArgExpression(scenarioStep.MultiLineTextArgument, paramToIdentifier));
				arguments.Add(GetTableArgExpression(scenarioStep.TableArg, testMethod.Statements, paramToIdentifier));
			}

			AddLineDirective(testMethod.Statements, scenarioStep);
			testMethod.Statements.Add
			(
				new CodeMethodInvokeExpression
				(
					testRunnerProperty,
					scenarioStep.GetType().Name,
					arguments.ToArray()
				)
			);
		}

		private IEnumerable<string> GetNonIgnoreTags(IEnumerable<Tag> tags)
		{
			return tags.Where(t => !t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t.Name);
		}

		private CodeExpression GetStringArrayExpression(Tags tags)
		{
			if (tags == null || tags.Count == 0)
				return new CodeCastExpression(typeof(string[]), new CodePrimitiveExpression(null));

			List<CodeExpression> items = new List<CodeExpression>();
			foreach (var tag in tags)
			{
				items.Add(new CodePrimitiveExpression(tag.Name));
			}

			return new CodeArrayCreateExpression(typeof(string[]), items.ToArray());
		}

		private CodeExpression GetStringArrayExpression(IEnumerable<string> items, ParameterSubstitution paramToIdentifier)
		{
			List<CodeExpression> expressions = new List<CodeExpression>();
			foreach (var item in items)
			{
				expressions.Add(GetSubstitutedString(item, paramToIdentifier));
			}

			return new CodeArrayCreateExpression(typeof(string[]), expressions.ToArray());
		}

		private class ParameterSubstitution : List<KeyValuePair<string, string>>
		{
			public void Add(string parameter, string identifier)
			{
				base.Add(new KeyValuePair<string, string>(parameter.Trim(), identifier));
			}

			public bool TryGetIdentifier(string param, out string id)
			{
				param = param.Trim();
				foreach (var pair in this)
				{
					if (pair.Key.Equals(param))
					{
						id = pair.Value;
						return true;
					}
				}
				id = null;
				return false;
			}
		}

		private bool CanUseFirstColumnAsName(Table table)
		{
			if (table.Header.Cells.Length == 0)
				return false;

			return table.Body.Select(r => r.Cells[0].Value.ToIdentifier()).Distinct().Count() == table.Body.Length;
		}

		private CodeMemberMethod GetTestMethodDeclaration(CodeTypeDeclaration testType, Scenario scenario)
		{
			CodeMemberMethod testMethod = new CodeMemberMethod()
			{
				Attributes = MemberAttributes.Public | MemberAttributes.Final,
				Name = string.Format(TEST_FORMAT, scenario.Title.ToIdentifier())
			};

			testType.Members.Add(testMethod);

			_testGeneratorProvider.SetTest(testMethod, scenario.Title);
			if (scenario.Tags != null)
			{
				_testGeneratorProvider.SetTestCategories(testMethod, GetNonIgnoreTags(scenario.Tags));
				if (scenario.Tags.Any(t => t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)))
				{
					_testGeneratorProvider.SetIgnore(testMethod);
				}
			}
			return testMethod;
		}

		private CodeExpression GetSubstitutedString(string text, ParameterSubstitution paramToIdentifier)
		{
			if (text == null)
				return new CodeCastExpression(typeof(string), new CodePrimitiveExpression(null));
			if (paramToIdentifier == null)
				return new CodePrimitiveExpression(text);

			Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");
			string formatText = text.Replace("{", "{{").Replace("}", "}}");
			List<string> arguments = new List<string>();

			formatText = paramRe.Replace(formatText, match =>
																							 {
																								 string param = match.Groups["param"].Value;
																								 string id;
																								 if (!paramToIdentifier.TryGetIdentifier(param, out id))
																									 return match.Value;
																								 int argIndex = arguments.IndexOf(id);
																								 if (argIndex < 0)
																								 {
																									 argIndex = arguments.Count;
																									 arguments.Add(id);
																								 }
																								 return "{" + argIndex + "}";
																							 });

			if (arguments.Count == 0)
				return new CodePrimitiveExpression(text);

			List<CodeExpression> formatArguments = new List<CodeExpression>();
			formatArguments.Add(new CodePrimitiveExpression(formatText));
			foreach (var id in arguments)
				formatArguments.Add(new CodeVariableReferenceExpression(id));

			return new CodeMethodInvokeExpression(
					new CodeTypeReferenceExpression(typeof(string)),
					"Format",
					formatArguments.ToArray());
		}

		private int tableCounter = 0;
		private CodeExpression GetTableArgExpression(Table tableArg, CodeStatementCollection statements, ParameterSubstitution paramToIdentifier)
		{
			if (tableArg == null)
				return new CodeCastExpression(TABLE_TYPE, new CodePrimitiveExpression(null));

			tableCounter++;

			//Table table0 = new Table(header...);
			var tableVar = new CodeVariableReferenceExpression("table" + tableCounter);
			statements.Add(
					new CodeVariableDeclarationStatement(TABLE_TYPE, tableVar.VariableName,
							new CodeObjectCreateExpression(
									TABLE_TYPE,
									GetStringArrayExpression(tableArg.Header.Cells.Select(c => c.Value), paramToIdentifier))));

			foreach (var row in tableArg.Body)
			{
				//table0.AddRow(cells...);
				statements.Add(
						new CodeMethodInvokeExpression(
								tableVar,
								"AddRow",
								GetStringArrayExpression(row.Cells.Select(c => c.Value), paramToIdentifier)));
			}
			return tableVar;
		}

		private CodeExpression GetMultilineTextArgExpression(string multiLineTextArgument, ParameterSubstitution paramToIdentifier)
		{
			return GetSubstitutedString(multiLineTextArgument, paramToIdentifier);
		}

		#region Line pragma handling

		private void AddLinePragmaInitial(CodeTypeDeclaration testType, Feature feature)
		{
			if (_allowDebugGeneratedFiles)
				return;

			testType.Members.Add(new CodeSnippetTypeMember(string.Format("#line 1 \"{0}\"", Path.GetFileName(feature.SourceFile))));
			testType.Members.Add(new CodeSnippetTypeMember("#line hidden"));
		}

		private void AddLineDirectiveHidden(CodeStatementCollection statements)
		{
			if (_allowDebugGeneratedFiles)
				return;

			statements.Add(new CodeSnippetStatement("#line hidden"));
		}

		private void AddLineDirective(CodeStatementCollection statements, Background background)
		{
			AddLineDirective(statements, null, background.FilePosition);
		}

		private void AddLineDirective(CodeStatementCollection statements, Scenario scenario)
		{
			AddLineDirective(statements, null, scenario.FilePosition);
		}

		private void AddLineDirective(CodeStatementCollection statements, ScenarioStep step)
		{
			AddLineDirective(statements, null, step.FilePosition);
		}

		private void AddLineDirective(CodeStatementCollection statements, string sourceFile, FilePosition filePosition)
		{
			if (filePosition == null || _allowDebugGeneratedFiles)
				return;

			if (sourceFile == null)
				statements.Add(new CodeSnippetStatement(
						string.Format("#line {0}", filePosition.Line)));
			else
				statements.Add(new CodeSnippetStatement(
						string.Format("#line {0} \"{1}\"", filePosition.Line, Path.GetFileName(sourceFile))));

			statements.Add(new CodeSnippetStatement(
							string.Format("//#indentnext {0}", filePosition.Column - 1)));
		}

		#endregion
	}
}
