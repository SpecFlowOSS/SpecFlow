using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Vs2010Integration.Commands;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace Vs2010IntegrationUnitTests
{
    class GoToDefinitionHelper
    {
        private readonly IEnumerable<MethodInfo> bindingMethods;

        public GoToDefinitionHelper(IEnumerable<MethodInfo> bindingMethods)
        {
            this.bindingMethods = bindingMethods;
        }

        private ITextSnapshotLine CreateTextSnapshotLine(GherkinBuffer gherkinBuffer, int lineNumber, ITextSnapshot snapshot)
        {
            var result = new Mock<ITextSnapshotLine>();
            result.Setup(x => x.LineNumber).Returns(lineNumber);
            result.Setup(x => x.GetText()).Returns(gherkinBuffer.GetContentFrom(lineNumber));
            result.Setup(x => x.Snapshot).Returns(snapshot);
            var lineStartPosition = gherkinBuffer.GetBufferPositionFromLine(lineNumber);
            result.Setup(x => x.Start)
                .Returns(new SnapshotPoint(snapshot, lineStartPosition));
            result.Setup(x => x.End)
                .Returns(new SnapshotPoint(snapshot, lineStartPosition + gherkinBuffer.GetLineEndPosition(lineNumber).LinePosition));
            return result.Object;
        }

        public IEnumerable<MethodInfo> GetMethodsMatchingTextAtCaret(GherkinBuffer gherkinBuffer, CaretPosition caretPosition)
        {
            var tracerMock = new Mock<IVisualStudioTracer>();
            tracerMock.Setup(x => x.Trace(It.IsAny<string>(), It.IsAny<string>())).Callback(
                (string message, string category) => Console.WriteLine(message));

            var projectScopeMock = CreateProjectScopeMock(tracerMock);

            var absoluteCaretPosition = gherkinBuffer.GetBufferPositionFromLine(caretPosition.LineNumber) +
                                        caretPosition.PositionInLine;

            var resultTextSnapshotMock = CreateTextSnapshotMock(gherkinBuffer, absoluteCaretPosition);

            var gherkinLanguageService = new GherkinLanguageService(projectScopeMock.Object, tracerMock.Object, false);
            var textViewMock = new Mock<IWpfTextView>();
            var caretMock = new Mock<ITextCaret>();
            var mappingPointMock = new Mock<IMappingPoint>();

            caretMock.Setup(x => x.Position).Returns(
                new Microsoft.VisualStudio.Text.Editor.CaretPosition(
                    new VirtualSnapshotPoint(resultTextSnapshotMock.Object, absoluteCaretPosition), mappingPointMock.Object, PositionAffinity.Successor));
            textViewMock.Setup(x => x.Caret).Returns(caretMock.Object);
            var gherkinEditorContext = new GherkinEditorContext(
                gherkinLanguageService, textViewMock.Object);

            const GherkinTextBufferChangeType type = new GherkinTextBufferChangeType();

            var change = new GherkinTextBufferChange(type, 0, 0, 0, 0, 0, 0, resultTextSnapshotMock.Object);
            gherkinLanguageService.TextBufferChanged(change);

	        var bindingMatchService =
		        new VsProjectScope.StepDefinitionMatchServiceWithOnlySimpleTypeConverter(CreateBindingRegistryMock().Object);

	        var resultHandler = new MatchingMethodResultHandler();
			GoToStepDefinitionCommand.GetMatchingMethods(gherkinEditorContext, bindingMatchService, null, resultHandler);
			var candidatingMatches = resultHandler.CandidatingMatches;

			if (candidatingMatches == null)
                return Enumerable.Empty<MethodInfo>();

			return from match in candidatingMatches
                   select match.StepBinding.Method.AssertMethodInfo();
        }

	    internal class MatchingMethodResultHandler : GoToStepDefinitionCommand.IMatchingMethodResultHandler
	    {
			public IEnumerable<BindingMatch> CandidatingMatches { get; private set; }

		    public void NoCurrentStep()
		    {
		    }

		    public void BindingServiceNotReady()
		    {
		    }

		    public void StepsFound(List<BindingMatch> candidatingMatches, BindingMatch bindingMatch)
		    {
			    CandidatingMatches = candidatingMatches;
		    }

		    public void NoMatchFound(CultureInfo bindingCulture, GherkinStep step)
		    {
		    }
	    }

	    private Mock<IBindingRegistry> CreateBindingRegistryMock()
        {
            var result = new Mock<IBindingRegistry>();
            result.Setup(x => x.Ready).Returns(true);

            var typeWithoutDefaultConstructor = typeof(string);
            var bindingFactory = ConfigurationServices.CreateInstance<IBindingFactory>(typeWithoutDefaultConstructor);

            var fakeBindingSourceProcessor = new FakeBindingSourceProcessor(bindingFactory);
            result.Setup(x => x.GetConsideredStepDefinitions(It.IsAny<StepDefinitionType>(), It.IsAny<string>()))
                .Returns(
                    (StepDefinitionType sdt, string stepText) =>
                        from method in bindingMethods
                        select
                            CreateStepDefinitionBinding(method, fakeBindingSourceProcessor));
            
			return result;
        }

        private static IStepDefinitionBinding CreateStepDefinitionBinding(MethodInfo method, FakeBindingSourceProcessor fakeBindingSourceProcessor)
        {
            var bindingRegistryBuilder = new RuntimeBindingRegistryBuilder(fakeBindingSourceProcessor);

            // TODO: change this method to call RuntimeBindingRegistryBuilder.BuildBindingsFromType
            // and this class to take a Type instead of a list of methods...
            // Also revert the changes made to the RuntimeBindingRegistryBuilder
            var bindingSourceMethod = bindingRegistryBuilder.CreateBindingSourceMethod(method);
            fakeBindingSourceProcessor.ProcessType(bindingRegistryBuilder.CreateBindingSourceType(method.DeclaringType));
            fakeBindingSourceProcessor.ProcessMethod(bindingSourceMethod);
            fakeBindingSourceProcessor.ProcessTypeDone();
            return fakeBindingSourceProcessor.LastStepDefinitionBinding;
        }

        private class FakeBindingSourceProcessor : BindingSourceProcessor
        {
            public FakeBindingSourceProcessor(IBindingFactory bindingFactory)
                : base(bindingFactory)
            {
            }

            public IStepDefinitionBinding LastStepDefinitionBinding { get; private set; }

            protected override void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
            {
                LastStepDefinitionBinding = stepDefinitionBinding;
            }

            protected override void ProcessHookBinding(IHookBinding hookBinding)
            {
                throw new NotImplementedException();
            }

	        protected override void ProcessStepArgumentTransformationBinding(
		        IStepArgumentTransformationBinding stepArgumentTransformationBinding)
	        {
	        }
        }

        private Mock<ITextSnapshot> CreateTextSnapshotMock(GherkinBuffer gherkinBuffer, int absoluteCaretPosition)
        {
            var resultTextSnapshotMock = new Mock<ITextSnapshot>();
            var editorText = gherkinBuffer.GetContent();

            resultTextSnapshotMock.Setup(x => x.GetText()).Returns(editorText);
            resultTextSnapshotMock.Setup(x => x.GetLineNumberFromPosition(absoluteCaretPosition));
            resultTextSnapshotMock.Setup(x => x.Length).Returns(editorText.Length);
            resultTextSnapshotMock.Setup(x => x.GetLineFromPosition(It.IsAny<int>())).Returns(
                (int position) =>
                {
                    var lineNumber = gherkinBuffer.GetLineNumberFromPosition(position);
                    return CreateTextSnapshotLine(gherkinBuffer, lineNumber, resultTextSnapshotMock.Object);
                });
            resultTextSnapshotMock.Setup(x => x.GetLineFromLineNumber(It.IsAny<int>())).Returns(
                (int lineNumber) => CreateTextSnapshotLine(gherkinBuffer, lineNumber, resultTextSnapshotMock.Object));
            resultTextSnapshotMock.Setup(x => x.LineCount).Returns(gherkinBuffer.LineCount);
            return resultTextSnapshotMock;
        }

        private static Mock<IProjectScope> CreateProjectScopeMock(Mock<IVisualStudioTracer> tracerMock)
        {
            var projectScopeMock = new Mock<IProjectScope>();
            var gherkinProcessingScheduler = new SynchroneousScheduler();
            projectScopeMock.Setup(x => x.GherkinProcessingScheduler)
                .Returns(gherkinProcessingScheduler);
            projectScopeMock.Setup(x => x.GherkinTextBufferParser)
                .Returns(new GherkinTextBufferParser(projectScopeMock.Object, tracerMock.Object));
            projectScopeMock.Setup(x => x.GherkinDialectServices)
                .Returns(new GherkinDialectServices(CultureInfo.CreateSpecificCulture("en-US")));
            var integrationOptionsProviderMock = new Mock<IIntegrationOptionsProvider>();
            integrationOptionsProviderMock.Setup(x => x.GetOptions()).Returns(new IntegrationOptions());
            projectScopeMock.Setup(x => x.IntegrationOptionsProvider).Returns(integrationOptionsProviderMock.Object);
            var classificationTypeRegistryServiceMock = new Mock<IClassificationTypeRegistryService>();
            classificationTypeRegistryServiceMock.Setup(x => x.GetClassificationType(It.IsAny<string>())).Returns(
                new Mock<IClassificationType>().Object);
            projectScopeMock.Setup(x => x.Classifications)
                .Returns(new GherkinFileEditorClassifications(classificationTypeRegistryServiceMock.Object));
            return projectScopeMock;
        }
    }

    internal class SynchroneousScheduler : IGherkinProcessingScheduler
    {
        public void Dispose()
        {
        }

        public void EnqueueParsingRequest(IGherkinProcessingTask change)
        {
            change.Apply();
        }

        public void EnqueueAnalyzingRequest(IGherkinProcessingTask task)
        {
            task.Apply();
        }
    }
}
