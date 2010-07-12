using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using java.util;
using gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    public class ListeningDoneException : Exception
    {
    }

    public class SyntaxColorer
    {
        public static IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan snapshotSpan, IClassificationTypeRegistryService registry)
        {
//            string fileContent = snapshotSpan.Snapshot.GetText(0, snapshotSpan.End.Position);
            string fileContent = snapshotSpan.Snapshot.GetText();
            var gherkinListener = new SyntaxColoringListener(snapshotSpan, registry);

            I18n languageService = new I18n("en");

            try
            {
                Lexer lexer = languageService.lexer(gherkinListener);
                lexer.scan(fileContent, null, 0);
                return gherkinListener.Classifications;
            }
            catch //(Exception ex)
            {
/*
                var errorClassificationType = registry.GetClassificationType("error");
                int startIndex = 0;
                if (gherkinListener.Classifications.Any())
                {
                    var last = gherkinListener.Classifications.Last();
                    startIndex = last.Span.Start + last.Span.Length;
                }
                    
                gherkinListener.Classifications.Add(new ClassificationSpan(
                    new SnapshotSpan(snapshotSpan.Snapshot, startIndex, snapshotSpan.Snapshot.Length - startIndex), 
                    errorClassificationType));
*/

                return gherkinListener.Classifications;
            }

        }
    }

    public class SyntaxColoringListener : Listener
    {
        public SnapshotSpan SnapshotSpan { get; set; }
        public List<ClassificationSpan> Classifications { get; private set; }
        private int startLine;
        private int endLine;

        private bool inScenarioOutline = false;
        private bool isInTable = false;

        private readonly IClassificationType keywordClassificationType;
        private readonly IClassificationType commentClassificationType;
        private readonly IClassificationType tagClassificationType;
        private readonly IClassificationType multilineTextClassificationType;
        private readonly IClassificationType placeholderClassificationType;
        private readonly IClassificationType scenarioTitleClassificationType;
        private readonly IClassificationType tableCellClassificationType;
        private readonly IClassificationType tableHeaderClassificationType;

        public SyntaxColoringListener(SnapshotSpan snapshotSpan, IClassificationTypeRegistryService registry)
        {
            SnapshotSpan = snapshotSpan;
            startLine = SnapshotSpan.Start.GetContainingLine().LineNumber;
            endLine = SnapshotSpan.End.GetContainingLine().LineNumber;

            Classifications = new List<ClassificationSpan>();

            keywordClassificationType = registry.GetClassificationType("keyword");
            commentClassificationType = registry.GetClassificationType("comment");
            tagClassificationType = registry.GetClassificationType("gherkin.tag");
            multilineTextClassificationType = registry.GetClassificationType("string");
            placeholderClassificationType = registry.GetClassificationType("gherkin.placeholder");
            scenarioTitleClassificationType = registry.GetClassificationType("gherkin.scenariotitle");
            tableCellClassificationType = registry.GetClassificationType("gherkin.tablecell");
            tableHeaderClassificationType = registry.GetClassificationType("gherkin.tableheader");
        }

        private int? GetEditorLine(int line)
        {
            var editorLine = line - 1;
//            if (editorLine > endLine)
//                throw new ListeningDoneException();
//            if (editorLine < startLine)
//                return null;
            return editorLine;
        }

        private string GetLineText(int line)
        {
            var editorLine = GetEditorLine(line);
            if (editorLine == null)
                return null;

            var snapshotLine = SnapshotSpan.Snapshot.GetLineFromLineNumber(editorLine.Value);
            return snapshotLine.GetText();
        }


        private void AddClassification(IClassificationType classificationType, int startIndex, int length)
        {
            Classifications.Add(
                new ClassificationSpan(
                    new SnapshotSpan(SnapshotSpan.Snapshot, new Span(startIndex, length)),
                    classificationType));
        }

        private void ColorizeLine(int line, IClassificationType classificationType)
        {
            var editorLine = GetEditorLine(line);
            if (editorLine == null)
                return;

            var snapshotLine = SnapshotSpan.Snapshot.GetLineFromLineNumber(editorLine.Value);
            AddClassification(classificationType, snapshotLine.Start, snapshotLine.LengthIncludingLineBreak);
        }

        private void ColorizeLinePart(string lineTextPart, int line, IClassificationType classificationType, int startIndex = 0)
        {
            var editorLine = GetEditorLine(line);
            if (editorLine == null)
                return;

            var snapshotLine = SnapshotSpan.Snapshot.GetLineFromLineNumber(editorLine.Value);

            int lineTextPartStartIndex = snapshotLine.GetText().IndexOf(lineTextPart, startIndex);
            if (lineTextPartStartIndex < 0)
                return;

            AddClassification(classificationType, snapshotLine.Start + lineTextPartStartIndex, lineTextPart.Length);
        }

        public void location(string str, int line)
        {
            
        }

        public void pyString(string text, int line)
        {
            int lineCount = text.Count(c => c.Equals('\n')) + 1 + 2;
            for (int currentLine = line; currentLine < line + lineCount; currentLine++)
            {
                ColorizeLine(currentLine, multilineTextClassificationType);
            }
        }

        public void feature(string keyword, string str2, string str3, int line)
        {
            RegisterKeyword(keyword, line);
        }

        public void background(string keyword, string str2, string str3, int line)
        {
            RegisterKeyword(keyword, line);
        }

        public void scenario(string keyword, string name, string description, int line)
        {
            inScenarioOutline = false;
            RegisterKeyword(keyword, line);
            ColorizeLinePart(name, line, scenarioTitleClassificationType);
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            inScenarioOutline = true;
            RegisterKeyword(keyword, line);
            ColorizeLinePart(name, line, scenarioTitleClassificationType);
        }

        public void examples(string keyword, string str2, string str3, int line)
        {
            RegisterKeyword(keyword, line);
        }


        private static readonly Regex placeholderRe = new Regex(@"\<.*?\>");
        public void step(string keyword, string text, int line)
        {
            isInTable = false;
            RegisterKeyword(keyword, line);
            if (inScenarioOutline)
            {
                var matches = placeholderRe.Matches(text);
                foreach (Match match in matches)
                    ColorizeLinePart(match.Value, line, placeholderClassificationType);
            }
        }

        private void RegisterKeyword(string keyword, int line)
        {
            ColorizeLinePart(keyword, line, keywordClassificationType);
        }

        public void comment(string commentText, int line)
        {
            ColorizeLine(line, commentClassificationType);
        }

        public void tag(string tagName, int line)
        {
            ColorizeLinePart(tagName, line, tagClassificationType);
        }

        public void eof()
        {
            
        }

        public void syntaxError(string str1, string str2, List l, int line)
        {
            
        }

        public void row(List list, int line)
        {
            var lineText = GetLineText(line);
            if (lineText == null)
                return;

            string[] cells = new string[list.size()];
            list.toArray(cells);

            IClassificationType classificationType = isInTable ? tableCellClassificationType : tableHeaderClassificationType;
            isInTable = true;

            int startIndex = lineText.IndexOf('|');
            if (startIndex < 0)
                return;
            foreach (var cell in cells)
            {
                ColorizeLinePart(cell.Trim(), line, classificationType, startIndex);
                startIndex = lineText.IndexOf('|', startIndex + 1);
                if (startIndex < 0)
                    return;
            }
        }
    }
}
