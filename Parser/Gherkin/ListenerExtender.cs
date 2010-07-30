using System.Linq;
using System.Text.RegularExpressions;
using gherkin;
using java.util;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    internal class ListenerExtender : Listener
    {
        private readonly I18n languageService;
        private readonly IGherkinListener gherkinListener;

        private ScenarioBlock lastScenarioBlock = ScenarioBlock.Given;
        private bool inTable = false;
        private bool afterFeature = false;

        public int LineOffset { get; set; }
        public GherkinBuffer GherkinBuffer { get; private set; }

        public IGherkinListener GherkinListener
        {
            get { return gherkinListener; }
        }

        private bool IsIncremental
        {
            get { return !GherkinBuffer.IsFullBuffer; }
        }

        private int GetEditorLine(int line)
        {
            return line - 1 + LineOffset;
        }

        private GherkinBufferPosition GetEOFPosition()
        {
            return GherkinBuffer.EndPosition;
        }

        private GherkinBufferSpan GetSingleLineSpan(int editorLine)
        {
            return GherkinBuffer.GetLineSpan(editorLine);
        }

        private GherkinBufferPosition GetLineStartPosition(int editorLine)
        {
            return GherkinBuffer.GetLineStartPosition(editorLine);
        }

        private static readonly Regex whitespaceOnlyRe = new Regex(@"^\s*$");
        private GherkinBufferSpan GetDescriptionSpan(int titleEditorLine, string description)
        {
            if (string.IsNullOrEmpty(description) || whitespaceOnlyRe.Match(description).Success)
                return null;

            int descriptionStartLine = titleEditorLine + 1;
            while (GherkinBuffer.GetMatchForLine(whitespaceOnlyRe, descriptionStartLine) != null)
                descriptionStartLine++;

            int lineCount = GetLineCount(description);
            return GherkinBuffer.GetLineRangeSpan(descriptionStartLine, descriptionStartLine + lineCount - 1);
        }

        static private readonly Regex cellSeparatorRe = new Regex(@"\|");
        private GherkinBufferSpan[] GetCellSpans(int editorLine, string[] cells)
        {
            var cellSeparatorPositions = 
                GherkinBuffer.GetMatchesForLine(cellSeparatorRe, editorLine).ToArray();

            var result = new GherkinBufferSpan[cells.Length];
            for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
            {
                if (cellSeparatorPositions.Length - 1 > cellIndex + 1)
                    break;

                result[cellIndex] = new GherkinBufferSpan(
                        cellSeparatorPositions[cellIndex].ShiftByCharacters(1),
                        cellSeparatorPositions[cellIndex + 1]);
            }

            return result;
        }

        private GherkinBufferSpan GetMultilineTextSpan(int editorLine, string text)
        {
            int lineCount = GetLineCount(text) + 2;
            return GherkinBuffer.GetLineRangeSpan(editorLine, editorLine + lineCount - 1);
        }

        static private readonly Regex newLineRe = new Regex(@"\r?\n");

        public ListenerExtender(I18n languageService, IGherkinListener gherkinListener, GherkinBuffer buffer)
        {
            this.languageService = languageService;
            this.gherkinListener = gherkinListener;
            this.GherkinBuffer = buffer;
        }

        private int GetLineCount(string text)
        {
            return newLineRe.Matches(text).Count + 1;
        }

        private StepKeyword GetStepKeyword(string keyword)
        {
            if (languageService.keywords("and").contains(keyword))
                return StepKeyword.And;
            // this is checked at the first place to interpret "*" as "and"

            if (languageService.keywords("given").contains(keyword))
                return StepKeyword.Given;

            if (languageService.keywords("when").contains(keyword))
                return StepKeyword.Given;

            if (languageService.keywords("then").contains(keyword))
                return StepKeyword.Then;

            if (languageService.keywords("but").contains(keyword))
                return StepKeyword.But;

            return StepKeyword.And; // if we dont find it, we suppose an "and"
        }

        private void ResetScenarioBlocks()
        {
            lastScenarioBlock = ScenarioBlock.Given;
        }

        private ScenarioBlock CalculateScenarioBlock(StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    lastScenarioBlock = ScenarioBlock.Given;
                    break;
                case StepKeyword.When:
                    lastScenarioBlock = ScenarioBlock.When;
                    break;
                case StepKeyword.Then:
                case StepKeyword.But:
                    lastScenarioBlock = ScenarioBlock.Then;
                    break;
                default:
                    // keep the existing one
                    break;
            }

            return lastScenarioBlock;
        }

        public void tag(string name, int line)
        {
            var editorLine = GetEditorLine(line);
            var bufferSpan = GetSingleLineSpan(editorLine);

            if (IsIncremental || afterFeature)
                gherkinListener.ScenarioTag(name, bufferSpan);
            else
                gherkinListener.FeatureTag(name, bufferSpan);
        }

        public void comment(string comment, int line)
        {
            var editorLine = GetEditorLine(line);
            var bufferSpan = GetSingleLineSpan(editorLine);
            gherkinListener.Comment(comment, bufferSpan);
        }

        public void location(string uri, int offset)
        {
            //TODO
        }

        public void feature(string keyword, string name, string description, int line)
        {
            afterFeature = false;

            var editorLine = GetEditorLine(line);
            var headerSpan = GetSingleLineSpan(editorLine);
            var descriptionSpan = GetDescriptionSpan(editorLine, description);
            gherkinListener.Feature(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void background(string keyword, string name, string description, int line)
        {
            ResetScenarioBlocks();

            var editorLine = GetEditorLine(line);
            var headerSpan = GetSingleLineSpan(editorLine);
            var descriptionSpan = GetDescriptionSpan(editorLine, description);
            gherkinListener.Background(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void scenario(string keyword, string name, string description, int line)
        {
            ResetScenarioBlocks();

            var editorLine = GetEditorLine(line);
            var headerSpan = GetSingleLineSpan(editorLine);
            var descriptionSpan = GetDescriptionSpan(editorLine, description);
            gherkinListener.Scenario(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            ResetScenarioBlocks();

            var editorLine = GetEditorLine(line);
            var headerSpan = GetSingleLineSpan(editorLine);
            var descriptionSpan = GetDescriptionSpan(editorLine, description);
            gherkinListener.ScenarioOutline(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void examples(string keyword, string name, string description, int line)
        {
            var editorLine = GetEditorLine(line);
            var headerSpan = GetSingleLineSpan(editorLine);
            var descriptionSpan = GetDescriptionSpan(editorLine, description);
            gherkinListener.Examples(keyword, name, description, headerSpan, descriptionSpan);
        }

        public void step(string keyword, string text, int line)
        {
            ResetStepArguments();

            var editorLine = GetEditorLine(line);
            var stepSpan = GetSingleLineSpan(editorLine);

            StepKeyword stepKeyword = GetStepKeyword(keyword);
            ScenarioBlock scenarioBlock = CalculateScenarioBlock(stepKeyword);

            gherkinListener.Step(keyword, stepKeyword, scenarioBlock, text, stepSpan);
        }

        private void ResetStepArguments()
        {
            inTable = false;
        }

        public void row(List cellList, int line)
        {
            string[] cells = new string[cellList.size()];
            cellList.toArray(cells);

            var editorLine = GetEditorLine(line);
            GherkinBufferSpan[] cellSpans = GetCellSpans(editorLine, cells);

            if (!inTable)
            {
                inTable = true;
                gherkinListener.TableHeader(cells, cellSpans);
            }
            else
            {
                gherkinListener.TableRow(cells, cellSpans);
            }
        }

        public void pyString(string text, int line)
        {
            var editorLine = GetEditorLine(line);
            GherkinBufferSpan textSpan = GetMultilineTextSpan(editorLine, text);
            gherkinListener.MultilineText(text, textSpan);
        }

        public void eof()
        {
            var eofPosition = GetEOFPosition();
            gherkinListener.EOF(eofPosition);
        }

        public void syntaxError(string state, string eventName, List legalEvents, int line)
        {
            //TODO
            var editorLine = GetEditorLine(line);
            var errorPosition = GetLineStartPosition(editorLine);

            string message = string.Format("Parser error. State: {0}, Event: {1}", state, eventName);

            gherkinListener.Error(message, errorPosition);
        }
    }
}