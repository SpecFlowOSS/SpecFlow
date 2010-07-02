using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using java.util;
using gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace GherkinFileClassifier
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
            catch (Exception ex)
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

        private readonly IClassificationType keywordClassificationType;
        private readonly IClassificationType commentClassificationType;
        private readonly IClassificationType tagClassificationType;
        private readonly IClassificationType multilineTextClassificationType;

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

        private void ColorizeLinePart(string lineTextPart, int line, IClassificationType classificationType)
        {
            var editorLine = GetEditorLine(line);
            if (editorLine == null)
                return;

            var snapshotLine = SnapshotSpan.Snapshot.GetLineFromLineNumber(editorLine.Value);

            int startIndex = snapshotLine.GetText().IndexOf(lineTextPart);
            if (startIndex < 0)
                return;

            AddClassification(classificationType, snapshotLine.Start + startIndex, lineTextPart.Length);
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

        public void scenario(string keyword, string str2, string str3, int line)
        {
            RegisterKeyword(keyword, line);
        }

        public void scenarioOutline(string keyword, string str2, string str3, int line)
        {
            RegisterKeyword(keyword, line);
        }

        public void examples(string keyword, string str2, string str3, int line)
        {
            RegisterKeyword(keyword, line);
        }

        public void step(string keyword, string text, int line)
        {
            RegisterKeyword(keyword, line);
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

        public void row(List l, int line)
        {
            
        }
    }
}
